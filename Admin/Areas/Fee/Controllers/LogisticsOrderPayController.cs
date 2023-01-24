using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Type;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Fee.Models.LogisticsOrderPay;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using shunda.Service.Customer;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.LogisticsOrderPay)]
public class LogisticsOrderPayController : Controller {
    private static readonly DateTime defaultDateBeginQuery = DateTime.ParseExact("2022-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
    private readonly CustomerCodeService customerCodeService;
    private readonly LogisticsOrderPayExcelService logisticsOrderPayExcelService;

    private readonly ISession session;

    public LogisticsOrderPayController(ISession session,
        CustomerCodeService customerCodeService,
        LogisticsOrderPayExcelService logisticsOrderPayExcelService
    ) {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.logisticsOrderPayExcelService = logisticsOrderPayExcelService;
    }

    [HttpGet]
    [Route("/admin/logistics/order/pay")]
    public IActionResult Index() {
        var model = new LogisticsOrderPayIndexModel();
        model.LogisticsModeTypeList = LogisticsModeType.GetList();

        return View("LogisticsOrderPayIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/pay/list")]
    public IActionResult List(LogisticsOrderPayListQueryParam queryParam) {
        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false) {
            var orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            int orderId = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCodeQuery)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();

            queryParam.OrderId = orderId;
        }

        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var logisticsOrderPayList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsOrderPayInfo>();

        foreach (var orderPay in logisticsOrderPayList) {
            // orderPay.Customer 肯定是非空的
            orderPay.CustomerCodeCache = customerCodeService.GetCustomerCode(orderPay.Customer.Id);
        }

        var model = new LogisticsOrderPayListModel(listPager);
        model.LogisticsOrderPayList = logisticsOrderPayList;

        return View("LogisticsOrderPayList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(LogisticsOrderPayListQueryParam queryParam) {
        var criteria = session.CreateCriteria<LogisticsOrderPayInfo>();
        // criteria = criteria.CreateAlias(nameof(LogisticsOrderPayInfo.Customer), nameof(LogisticsOrderPayInfo.Customer));
        // criteria = criteria.CreateAlias(nameof(LogisticsOrderPayInfo.Order), nameof(LogisticsOrderPayInfo.Order));
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.Status == PayStatus.Paid));

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false) {
            var customerCodeQuery = queryParam.CustomerCodeQuery.Trim();

            var customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerCodeQuery);
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.Customer.Id == customerId));
        }

        if (queryParam.OrderId != null) {
            var orderId = queryParam.OrderId.Value;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.Order.Id == orderId));
        }

        var firstModeType = LogisticsModeType.GetById(queryParam.LogisticsModeTypeIdQuery ?? 0);
        if (firstModeType != null && firstModeType != LogisticsModeType.None) {
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsOrderPayInfo.Order)}.{nameof(LogisticsOrderPayInfo.Order.FirstModeType)}", firstModeType));
        }

        if (queryParam.PayTimeBeginQuery != null) {
            var payTimeBeginQuery = queryParam.PayTimeBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.CreateTime >= payTimeBeginQuery));
        }

        if (queryParam.PayTimeEndQuery != null) {
            var payTimeEndQuery = queryParam.PayTimeEndQuery.Value.Date;
            payTimeEndQuery = payTimeEndQuery.AddDays(1);

            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.CreateTime < payTimeEndQuery));
        }

        return criteria;
    }

    [NonAction]
    private IList<LogisticsOrderPayDateStatItem> GetDateStatDataList(LogisticsOrderPayListQueryParam queryParam) {
        var criteria = GetListCriteria(queryParam); // 这里面会把 queryParam.PayTimeEndQuery 加上 1
        var alias = criteria.Alias + "_"; // 感觉这里很牵强

        var payDateStatObjArrayList = criteria
            .SetProjection(Projections.ProjectionList()
                .Add(Projections.SqlProjection($"SUM(IF({alias}.pay_type_id = {PayType.AliPay.Id}, {alias}.amount, 0)) AS alipay_amount", new[] { "alipay_amount" }, new IType[] { NHibernateUtil.Decimal }))
                .Add(Projections.SqlProjection($"SUM(IF({alias}.pay_type_id = {PayType.Wechat.Id}, {alias}.amount, 0)) AS wechat_amount", new[] { "wechat_amount" }, new IType[] { NHibernateUtil.Decimal }))
                .Add(Projections.SqlProjection($"SUM(IF({alias}.pay_type_id = {PayType.PayPal.Id}, {alias}.amount, 0)) AS paypal_amount", new[] { "paypal_amount" }, new IType[] { NHibernateUtil.Decimal }))
                .Add(Projections.SqlProjection($"SUM(IF({alias}.pay_type_id = {PayType.Balance.Id}, {alias}.amount, 0)) AS balance_amount", new[] { "balance_amount" }, new IType[] { NHibernateUtil.Decimal }))
                .Add(Projections.SqlProjection($"SUM(IF({alias}.pay_type_id = {PayType.WriteOff.Id}, {alias}.amount, 0)) AS write_off_amount", new[] { "write_off_amount" }, new IType[] { NHibernateUtil.Decimal }))
                .Add(Projections.SqlProjection($"SUM({alias}.amount) AS total_amount", new[] { "total_amount" }, new IType[] { NHibernateUtil.Decimal }))
                .Add(Projections.SqlGroupProjection($"DATE({alias}.create_time) AS create_date", $"DATE({alias}.create_time)", new[] { "create_date" }, new IType[] { NHibernateUtil.Date }))
            )
            .AddOrder(Order.Desc(Projections.SqlGroupProjection($"DATE({alias}.create_time) AS create_date", $"DATE({alias}.create_time)", new[] { "create_date" }, new IType[] { NHibernateUtil.Date })))
            .List<object[]>();

        IList<LogisticsOrderPayDateStatItem> payDateStatList = new List<LogisticsOrderPayDateStatItem>();
        foreach (var payDateStatObjArray in payDateStatObjArrayList)
            payDateStatList.Add(new LogisticsOrderPayDateStatItem {
                AlipayAmount = (decimal)payDateStatObjArray[0],
                WechatAmount = (decimal)payDateStatObjArray[1],
                PaypalAmount = (decimal)payDateStatObjArray[2],
                BalanceAmount = (decimal)payDateStatObjArray[3],
                WriteOffAmount = (decimal)payDateStatObjArray[4],
                TotalAmount = (decimal)payDateStatObjArray[5],
                Date = (DateTime)payDateStatObjArray[6]
            });

        var dateBegin = defaultDateBeginQuery;
        if (queryParam.PayTimeBeginQuery != null) dateBegin = queryParam.PayTimeBeginQuery.Value;

        var dateEnd = DateTime.Today;
        if (queryParam.PayTimeEndQuery != null) dateEnd = queryParam.PayTimeEndQuery.Value;

        // 数据空白日期填充
        for (var date = dateEnd; date >= dateBegin; date = date.AddDays(-1)) {
            var payDateStat = payDateStatList.Where(e => e.Date == date).Take(1).SingleOrDefault(); // 可以优化下效率
            if (payDateStat == null) {
                payDateStat = new LogisticsOrderPayDateStatItem { Date = date };
                payDateStatList.Add(payDateStat);
            }
        }

        payDateStatList = payDateStatList.OrderByDescending(e => e.Date).ToList();

        return payDateStatList;
    }

    [HttpPost]
    [Route("/admin/logistics/order/pay/date/stat/list")]
    public IActionResult DateStatList(LogisticsOrderPayListQueryParam queryParam) {
        var dateBegin = defaultDateBeginQuery;
        if (queryParam.PayTimeBeginQuery != null) dateBegin = queryParam.PayTimeBeginQuery.Value;

        var dateEnd = DateTime.Today;
        if (queryParam.PayTimeEndQuery != null) dateEnd = queryParam.PayTimeEndQuery.Value;

        var totalCount = (int)((dateEnd - dateBegin).TotalDays + 1);

        queryParam.PayTimeBeginQuery = dateBegin;
        var calculateDateBegin = dateEnd.AddDays(1 - (queryParam.PageIndex + 1) * queryParam.PageSize);
        if (calculateDateBegin > dateBegin) queryParam.PayTimeBeginQuery = calculateDateBegin;

        queryParam.PayTimeEndQuery = dateEnd.AddDays(-queryParam.PageIndex * queryParam.PageSize);

        var listPager = new ListPager(queryParam, totalCount);
        var model = new LogisticsOrderPayDateStatListModel(listPager);
        model.DateStatList = GetDateStatDataList(queryParam);

        return View("LogisticsOrderPayDateStatList", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/pay/export")]
    public IActionResult Export(LogisticsOrderPayListQueryParam queryParam) {
        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false) {
            var orderCodeQuery = queryParam.OrderCodeQuery.Trim();

            int orderId = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCodeQuery)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();

            queryParam.OrderId = orderId;
        }

        // order pay list
        var orderPayList = GetListCriteria(queryParam)
            .AddOrder(Order.Asc(nameof(Entity.Id)))
            .List<LogisticsOrderPayInfo>();

        var dateBegin = defaultDateBeginQuery;
        if (queryParam.PayTimeBeginQuery != null) dateBegin = queryParam.PayTimeBeginQuery.Value;

        var dateEnd = DateTime.Today;
        if (queryParam.PayTimeEndQuery != null) dateEnd = queryParam.PayTimeEndQuery.Value;

        queryParam.PayTimeBeginQuery = dateBegin;
        queryParam.PayTimeEndQuery = dateEnd;

        // date stat list
        var dateStatList = GetDateStatDataList(queryParam);

        // export
        var fileName = "实时收入" + ".xlsx";
        var excel = logisticsOrderPayExcelService.Export(orderPayList, dateStatList);

        return File(excel, ContentType.Xlsx, fileName);
    }
}
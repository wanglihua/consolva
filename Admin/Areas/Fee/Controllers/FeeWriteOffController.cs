using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Fee.Models.FeeWriteOff;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using shunda.Service.Customer;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.WriteOffFee)]
public class FeeWriteOffController : Controller {
    private readonly ISession session;
    private readonly CustomerCodeService customerCodeService;
    private readonly LogisticsOrderFeeService logisticsOrderFeeService;
    private readonly FeeWriteOffExcelService feeWriteOffExcelService;

    public FeeWriteOffController(ISession session,
        CustomerCodeService customerCodeService,
        LogisticsOrderFeeService logisticsOrderFeeService,
        FeeWriteOffExcelService feeWriteOffExcelService
    ) {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.logisticsOrderFeeService = logisticsOrderFeeService;
        this.feeWriteOffExcelService = feeWriteOffExcelService;
    }

    [HttpGet]
    [Route("/admin/fee/write/off")]
    public IActionResult Index() {
        FeeWriteOffIndexModel model = new FeeWriteOffIndexModel();

        model.FeeReasonList = session.Query<ManualFeeReasonInfo>().Where(e => e.FeeType == ManualFeeType.WriteOffFee).ToList();

        return View("FeeWriteOffIndex", model);
    }

    [HttpPost]
    [Route("/admin/fee/write/off/list")]
    public IActionResult List(FeeWriteOffListQueryParam queryParam) {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsOrderPayInfo> logisticsOrderPayList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsOrderPayInfo>();

        foreach (LogisticsOrderPayInfo orderPay in logisticsOrderPayList) {
            orderPay.CustomerCodeCache = customerCodeService.GetCustomerCode(orderPay.Customer.Id);
        }

        FeeWriteOffListModel model = new FeeWriteOffListModel(listPager);
        model.LogisticsOrderPayList = logisticsOrderPayList;

        // 汇总信息
        var sumData = GetListCriteria(queryParam)
            .SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count(Projections.Distinct(Projections.Property($"{nameof(LogisticsOrderPayInfo.Customer)}.{nameof(LogisticsOrderPayInfo.Customer.Id)}"))))
                    .Add(Projections.Count(Projections.Distinct(Projections.Property($"{nameof(LogisticsOrderPayInfo.Order)}.{nameof(LogisticsOrderPayInfo.Order.Id)}"))))
                    .Add(Projections.Sum(Projections.Property(nameof(LogisticsOrderPayInfo.Amount))))
            )
            .UniqueResult<object[]>();

        model.CustomerCount = (int)sumData[0];
        model.OrderCount = (int)sumData[1];
        model.AmountSum = (decimal)(sumData[2] ?? 0m);

        return View("FeeWriteOffList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(FeeWriteOffListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<LogisticsOrderPayInfo>();

        criteria = criteria.SetReadOnly(true);

        // criteria = criteria.CreateAlias(nameof(LogisticsOrderPayInfo.Order), nameof(LogisticsOrderPayInfo.Order));

        criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.Status == PayStatus.Paid && e.PayType == PayType.WriteOff));

        if (queryParam.ManualReasonIdQuery != 0) {
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.WriteOffReasonId == queryParam.ManualReasonIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false) {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsOrderPayInfo.Order)}.{nameof(LogisticsOrderPayInfo.Order.Code)}", orderCodeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false) {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsOrderPayInfo.Customer)}.{nameof(LogisticsOrderPayInfo.Customer.Id)}", customerIdQuery));
        }

        if (queryParam.BeginDateQuery != null) {
            DateTime beginDateQueryValue = queryParam.BeginDateQuery.Value;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.UpdateTime >= beginDateQueryValue)); // 这里 CreateTime 和 UpdateTime 都可以
        }

        if (queryParam.EndDateQuery != null) {
            DateTime endDateQueryValue = queryParam.EndDateQuery.Value.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.UpdateTime < endDateQueryValue)); // 这里 CreateTime 和 UpdateTime 都可以
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/fee/write/off/export")]
    public IActionResult Export(FeeWriteOffListQueryParam queryParam) {
        IList<LogisticsOrderPayInfo> feeWriteOffList = GetListCriteria(queryParam)
            .AddOrder(new Order($"{nameof(LogisticsOrderPayInfo.Id)}", true))
            .List<LogisticsOrderPayInfo>();

        string fileName = "费用核销.xlsx";
        var excel = feeWriteOffExcelService.Export(feeWriteOffList);
        return File(excel, ContentType.Xlsx, fileName);
    }

    /// <summary>
    /// 这个方法原来是更新历史数据的，现在基本没用了
    /// </summary>
    /// <returns></returns>
    [NoLoginRequired]
    [HttpPost]
    [Route("/admin/fee/write/off/update/all/write/off/order/pay/status")]
    public IActionResult UpdateAllWriteOffOrderPayStatus() {
        var writeOffOrderList = session.Query<LogisticsOrderPayInfo>()
            .Where(e => e.PayType == PayType.WriteOff)
            .Select(e => e.Order)
            .Distinct()
            .ToList();

        foreach (LogisticsOrderInfo logisticsOrder in writeOffOrderList) {
            logisticsOrderFeeService.SumFeeAndUpdatePayStatus(logisticsOrder);
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "更新核销订单支付状态成功！" }
        });
    }
}
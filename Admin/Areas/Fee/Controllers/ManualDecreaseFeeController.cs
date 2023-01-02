using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Fee.Models.ManualDecreaseFee;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Fee;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.ManualDecreaseFee)]
public class ManualDecreaseFeeController : Controller {
    private readonly ISession session;
    private readonly CustomerCodeService customerCodeService;
    private readonly ManualDecreaseFeeExcelService manualDecreaseFeeExcelService;

    public ManualDecreaseFeeController(ISession session,
        CustomerCodeService customerCodeService,
        ManualDecreaseFeeExcelService manualDecreaseFeeExcelService) {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.manualDecreaseFeeExcelService = manualDecreaseFeeExcelService;
    }

    [HttpGet]
    [Route("/admin/manual/decrease/fee")]
    public IActionResult Index() {
        ManualDecreaseFeeIndexModel model = new ManualDecreaseFeeIndexModel();

        model.FeeReasonList = session.Query<ManualFeeReasonInfo>().Where(e => e.FeeType == ManualFeeType.ManualDecrease).ToList();

        return View("ManualDecreaseFeeIndex", model);
    }

    [HttpPost]
    [Route("/admin/manual/decrease/fee/list")]
    public IActionResult List(ManualDecreaseFeeListQueryParam queryParam) {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsOrderFeeInfo> logisticsOrderFeeList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsOrderFeeInfo>();

        foreach (LogisticsOrderFeeInfo orderFee in logisticsOrderFeeList) {
            orderFee.CustomerCodeCache = customerCodeService.GetCustomerCode(orderFee.Customer.Id);
        }

        ManualDecreaseFeeListModel model = new ManualDecreaseFeeListModel(listPager);
        model.LogisticsOrderFeeList = logisticsOrderFeeList;

        // 汇总信息
        var sumData = GetListCriteria(queryParam)
            .SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count(Projections.Distinct(Projections.Property($"{nameof(LogisticsOrderFeeInfo.Customer)}.{nameof(LogisticsOrderFeeInfo.Customer.Id)}"))))
                    .Add(Projections.Count(Projections.Distinct(Projections.Property($"{nameof(LogisticsOrderFeeInfo.Order)}.{nameof(LogisticsOrderFeeInfo.Order.Id)}"))))
                    .Add(Projections.Sum(Projections.Property(nameof(LogisticsOrderFeeInfo.Fee))))
            )
            .UniqueResult<object[]>();

        model.CustomerCount = (int)sumData[0];
        model.OrderCount = (int)sumData[1];
        model.FeeSum = (decimal)(sumData[2] ?? 0m);

        return View("ManualDecreaseFeeList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ManualDecreaseFeeListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<LogisticsOrderFeeInfo>();

        // criteria = criteria.CreateAlias(nameof(LogisticsOrderFeeInfo.Order), nameof(LogisticsOrderFeeInfo.Order));
        // criteria = criteria.CreateAlias(nameof(LogisticsOrderFeeInfo.Customer), nameof(LogisticsOrderFeeInfo.Customer));

        criteria = criteria.Add(Restrictions.Where<LogisticsOrderFeeInfo>(e => e.Type == LogisticsOrderFeeType.ManualDecrease));

        criteria = criteria.SetReadOnly(true);

        if (queryParam.ManualReasonIdQuery != 0) {
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderFeeInfo>(e => e.ManualReason!.Id == queryParam.ManualReasonIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false) {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsOrderFeeInfo.Order)}.{nameof(LogisticsOrderFeeInfo.Order.Code)}", orderCodeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false) {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsOrderFeeInfo.Customer)}.{nameof(LogisticsOrderFeeInfo.Customer.Id)}", customerIdQuery));
        }

        if (queryParam.BeginDateQuery != null) {
            DateTime beginDateQueryValue = queryParam.BeginDateQuery.Value;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderFeeInfo>(e => e.CreateTime >= beginDateQueryValue));
        }

        if (queryParam.EndDateQuery != null) {
            DateTime endDateQueryValue = queryParam.EndDateQuery.Value.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderFeeInfo>(e => e.CreateTime < endDateQueryValue));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/manual/decrease/fee/export")]
    public IActionResult Export(ManualDecreaseFeeListQueryParam queryParam) {
        IList<LogisticsOrderFeeInfo> feeList = GetListCriteria(queryParam)
            .AddOrder(new Order($"{nameof(LogisticsOrderFeeInfo.Id)}", true))
            .List<LogisticsOrderFeeInfo>();

        foreach (LogisticsOrderFeeInfo orderFee in feeList) {
            orderFee.CustomerCodeCache = customerCodeService.GetCustomerCode(orderFee.Customer.Id);
        }

        string fileName = "减少费用.xlsx";
        var excel = manualDecreaseFeeExcelService.Export(feeList);

        return File(excel, ContentType.Xlsx, fileName);
    }
}
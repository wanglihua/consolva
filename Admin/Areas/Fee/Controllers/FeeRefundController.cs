using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Fee.Models.FeeRefund;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Fee;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using shunda.Service.Customer;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.FeeRefund)]
public class FeeRefundController : Controller {
    private readonly ISession session;
    private readonly CustomerCodeService customerCodeService;
    private readonly FeeRefundExcelService feeRefundExcelService;

    public FeeRefundController(ISession session,
        CustomerCodeService customerCodeService,
        FeeRefundExcelService feeRefundExcelService
    ) {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.feeRefundExcelService = feeRefundExcelService;
    }

    [HttpGet]
    [Route("/admin/fee/refund")]
    public IActionResult Index() {
        FeeRefundIndexModel model = new FeeRefundIndexModel();

        model.TypeList = FeeRefundType.GetList();
        model.StatusList = FeeRefundStatus.GetList();

        return View("FeeRefundIndex", model);
    }

    [HttpPost]
    [Route("/admin/fee/refund/list")]
    public IActionResult List(FeeRefundListQueryParam queryParam) {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<FeeRefundInfo> logisticsOrderRefundList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<FeeRefundInfo>();

        foreach (FeeRefundInfo feeRefund in logisticsOrderRefundList) {
            feeRefund.CustomerCodeCache = customerCodeService.GetCustomerCode(feeRefund.Customer.Id);
        }

        FeeRefundListModel model = new FeeRefundListModel(listPager);
        model.FeeRefundList = logisticsOrderRefundList;

        // 汇总信息
        var sumData = GetListCriteria(queryParam)
            .SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count(Projections.Distinct(Projections.Property($"{nameof(FeeRefundInfo.Customer)}.{nameof(FeeRefundInfo.Customer.Id)}"))))
                    .Add(Projections.Count(Projections.Distinct(Projections.Property($"{nameof(FeeRefundInfo.Order)}.{nameof(FeeRefundInfo.Order.Id)}"))))
                    .Add(Projections.Sum(Projections.Property(nameof(FeeRefundInfo.Amount))))
            )
            .UniqueResult<object[]>();

        model.CustomerCount = (int)sumData[0];
        model.OrderCount = (int)sumData[1];
        model.AmountSum = (decimal)(sumData[2] ?? 0m);

        return View("FeeRefundList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(FeeRefundListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<FeeRefundInfo>();

        criteria.SetReadOnly(true);

        // criteria = criteria.CreateAlias(nameof(FeeRefundInfo.Customer), nameof(FeeRefundInfo.Customer));
        // criteria = criteria.CreateAlias(nameof(FeeRefundInfo.Order), nameof(FeeRefundInfo.Order));

        if (queryParam.TypeQuery != null && queryParam.TypeQuery != FeeRefundType.None) {
            criteria = criteria.Add(Restrictions.Where<FeeRefundInfo>(e => e.Type == queryParam.TypeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false) {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"{nameof(FeeRefundInfo.Order)}.{nameof(FeeRefundInfo.Order.Code)}", orderCodeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false) {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Eq($"{nameof(FeeRefundInfo.Customer)}.{nameof(FeeRefundInfo.Customer.Id)}", customerIdQuery));
        }


        if (string.IsNullOrWhiteSpace(queryParam.ReasonQuery) == false) {
            string reasonQuery = queryParam.ReasonQuery.Trim();
            criteria = criteria.Add(Restrictions.On<FeeRefundInfo>(e => e.ReasonContent).IsInsensitiveLike(reasonQuery, MatchMode.Anywhere));
        }

        if (queryParam.StatusQuery != null && queryParam.StatusQuery != FeeRefundStatus.None) {
            criteria = criteria.Add(Restrictions.Where<FeeRefundInfo>(e => e.Status == queryParam.StatusQuery));
        }

        if (queryParam.BeginDateQuery != null) {
            DateTime beginDateQueryValue = queryParam.BeginDateQuery.Value;
            criteria = criteria.Add(Restrictions.Where<FeeRefundInfo>(e => e.CreateTime >= beginDateQueryValue)); // 这里 CreateTime 和 UpdateTime 都可以
        }

        if (queryParam.EndDateQuery != null) {
            DateTime endDateQueryValue = queryParam.EndDateQuery.Value.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<FeeRefundInfo>(e => e.CreateTime < endDateQueryValue)); // 这里 CreateTime 和 UpdateTime 都可以
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/fee/refund/export")]
    public IActionResult Export(FeeRefundListQueryParam queryParam) {
        IList<FeeRefundInfo> refundList = GetListCriteria(queryParam)
            .AddOrder(new Order($"{nameof(FeeRefundInfo.Id)}", true))
            .List<FeeRefundInfo>();

        string fileName = "退赔款.xlsx";
        var excel = feeRefundExcelService.Export(refundList);

        return File(excel, ContentType.Xlsx, fileName);
    }
}
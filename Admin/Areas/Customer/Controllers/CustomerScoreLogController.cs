using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Customer.Models.CustomerScoreLog;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerScoreLog)]
public class CustomerScoreLogController : Controller
{
    private readonly CustomerCodeService customerCodeService;
    private readonly ISession session;

    public CustomerScoreLogController(ISession session, CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/customer/score/log")]
    public IActionResult Index()
    {
        var model = new CustomerScoreLogIndexModel();
        model.ScoreTypeList = CustomerScoreType.GetList();

        return View("CustomerScoreLogIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/score/log/list")]
    public IActionResult List(CustomerScoreLogListQueryParam queryParam)
    {
        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            var orderCode = queryParam.OrderCodeQuery.Trim();
            var orderId = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCode)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();

            queryParam.OrderId = orderId;
        }

        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var customerScoreLogList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerScoreLogInfo>();

        foreach (CustomerScoreLogInfo scoreLog in customerScoreLogList)
        {
            scoreLog.CustomerCodeCache = customerCodeService.GetCustomerCode(scoreLog.Customer.Id);
            scoreLog.LogisticsOrderCustomerCodeCache = customerCodeService.GetCustomerCode(scoreLog.LogisticsOrderCustomer?.Id ?? 0);
        }

        var model = new CustomerScoreLogListModel(listPager);
        model.CustomerScoreLogList = customerScoreLogList;

        return View("CustomerScoreLogList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerScoreLogListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<CustomerScoreLogInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            var customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Where<CustomerScoreLogInfo>(e => e.Customer.Id == customerIdQuery));
        }

        if (queryParam.ScoreTypeQuery != null && queryParam.ScoreTypeQuery != CustomerScoreType.None)
        {
            criteria = criteria.Add(Restrictions.Where<CustomerScoreLogInfo>(e => e.ScoreType == queryParam.ScoreTypeQuery));
        }

        if (queryParam.OrderId != null)
        {
            var orderId = queryParam.OrderId;
            criteria = criteria.Add(Restrictions.Where<CustomerScoreLogInfo>(e => e.LogisticsOrder!.Id == orderId));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/customer/score/export")]
    public IActionResult Export() {
        // var fileName = "超级VIP折扣.xlsx";
        // byte[] excel = customerSuperVipDiscountExcelService.Export();
        // return File(excel, ContentType.Xlsx, fileName);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [HttpPost]
    [Route("/admin/customer/score/import")]
    [Transactional]
    public IActionResult Import(IFormFile excelFile) {
        // UserInfo loginUser = loginUserService.GetLoginUser();
        // customerSuperVipDiscountExcelService.Import(excelFile, loginUser);

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "导入成功！" }
        });
    }    
}
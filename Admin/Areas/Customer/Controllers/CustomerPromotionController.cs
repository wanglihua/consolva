using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Admin.Areas.Customer.Models.CustomerPromotion;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

/// <summary>
/// 客户推广 控制器
/// </summary>
[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerPromotion)]
public class CustomerPromotionController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly CustomerCodeService customerCodeService;
    private readonly CustomerPromotionService customerPromotionService;
    private readonly CustomerPromotionExcelService customerPromotionExcelService;

    public CustomerPromotionController(
        ISession session,
        CustomerCodeService customerCodeService,
        CustomerPromotionService customerPromotionService,
        CustomerPromotionExcelService customerPromotionExcelService
    ) {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.customerPromotionService = customerPromotionService;
        this.customerPromotionExcelService = customerPromotionExcelService;
    }

    [HttpGet]
    [Route("/admin/customer/customer/promotion")]
    public IActionResult Index() {
        CustomerPromotionIndexModel model = new CustomerPromotionIndexModel();
        return View("CustomerPromotionIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/customer/promotion/list")]
    public IActionResult List(CustomerPromotionListQueryParam queryParam) {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<CustomerPromotionInfo> customerPromotionList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerPromotionInfo>();

        CustomerPromotionListModel model = new CustomerPromotionListModel(listPager);
        model.CustomerPromotionList = customerPromotionList;
        model.ParentAlias = parentAlias;
        model.ChildAlias = childAlias;

        return View("CustomerPromotionList", model);
    }

    private static readonly string parentAlias = "p";
    private static readonly string childAlias = "c";

    [NonAction]
    private ICriteria GetListCriteria(CustomerPromotionListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<CustomerPromotionInfo>();
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(CustomerPromotionInfo.Parent), parentAlias);
        criteria = criteria.CreateAlias(nameof(CustomerPromotionInfo.Child), childAlias, JoinType.LeftOuterJoin); // 其实这里已经可以保证 Child 肯定不会为空

        // criteria = criteria.Add(Restrictions.Where<CustomerPromotionInfo>(e => e.Child != null)); // Child 不为空

        if (string.IsNullOrWhiteSpace(queryParam.ParentCustomerCode) == false) {
            var parentId = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.ParentCustomerCode);
            criteria = criteria.Add(Restrictions.Eq($"{parentAlias}.{nameof(CustomerInfo.Id)}", parentId));
        }

        if (string.IsNullOrWhiteSpace(queryParam.ChildCustomerCode) == false) {
            var childId = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.ChildCustomerCode);
            criteria = criteria.Add(Restrictions.Eq($"{childAlias}.{nameof(CustomerInfo.Id)}", childId));
        }

        var childOpenId = queryParam.ChildOpenId;
        if (string.IsNullOrWhiteSpace(childOpenId) == false) {
            criteria = criteria.Add(Restrictions.On<CustomerPromotionInfo>(e => e.OpenId).IsInsensitiveLike(childOpenId, MatchMode.Anywhere));
        }

        if (queryParam.BeginDateQuery != null) {
            DateTime beginDateQueryValue = queryParam.BeginDateQuery.Value;
            criteria = criteria.Add(Restrictions.Where<CustomerPromotionInfo>(e => e.CreateTime >= beginDateQueryValue));
        }

        if (queryParam.EndDateQuery != null) {
            DateTime endDateQueryValue = queryParam.EndDateQuery.Value.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<CustomerPromotionInfo>(e => e.CreateTime < endDateQueryValue));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/customer/promotion/update/child/pay/amount")]
    [Transactional]
    public IActionResult UpdateChildPayAmount() {
        customerPromotionService.UpdateChildPayAmount();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "更新下家支付总额成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/customer/promotion/export")]
    // [Transactional]
    public IActionResult Export(CustomerPromotionListQueryParam queryParam) {
        IList<CustomerPromotionInfo> customerPromotionList = GetListCriteria(queryParam)
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .List<CustomerPromotionInfo>();

        string fileName = "会员推广.xlsx";
        byte[] excelContent = customerPromotionExcelService.GetExcelContent(customerPromotionList);
        return File(excelContent, ContentType.Xlsx, fileName);
    }
}
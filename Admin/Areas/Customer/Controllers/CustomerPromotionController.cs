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
    private CustomerPromotionService customerPromotionService;

    public CustomerPromotionController(
        ISession session,
        CustomerPromotionService customerPromotionService
    ) {
        this.session = session;
        this.customerPromotionService = customerPromotionService;
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
        int totalCount = GetQueryOver(queryParam)
            // .Select(Projections.Count<CustomerPromotionInfo>(e => customerPromotionAlias!.Id))
            .Select(Projections.RowCount())
            .SingleOrDefault<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        var listQuery = GetQueryOver(queryParam);
        if (listPager.IsAscending) {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Asc;
        } else {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Desc;
        }

        listQuery = listQuery.ThenByAlias(() => customerPromotionAlias!.Id).Desc;

        IList<CustomerPromotionInfo> customerPromotionList = listQuery
            .Skip(listPager.Skip())
            .Take(listPager.Take())
            .List<CustomerPromotionInfo>();

        /*
        foreach (CustomerPromotionInfo customerPromotion in customerPromotionList) {
        }
        */

        CustomerPromotionListModel model = new CustomerPromotionListModel(listPager);
        model.CustomerPromotionList = customerPromotionList;

        model.CustomerPromotionAlias = nameof(customerPromotionAlias);
        model.ParentAlias = nameof(parentAlias);
        model.ChildAlias = nameof(childAlias);

        return View("CustomerPromotionList", model);
    }

    private CustomerPromotionInfo? customerPromotionAlias = null;
    private CustomerInfo? parentAlias = null;
    private CustomerInfo? childAlias = null;

    [NonAction]
    private IQueryOver<CustomerPromotionInfo, CustomerPromotionInfo> GetQueryOver(CustomerPromotionListQueryParam queryParam) {
        var query = session.QueryOver<CustomerPromotionInfo>(() => customerPromotionAlias!)
            .JoinAlias(() => customerPromotionAlias!.Parent, () => parentAlias)
            .JoinAlias(() => customerPromotionAlias!.Child, () => childAlias, JoinType.LeftOuterJoin);

        return query;
    }
}
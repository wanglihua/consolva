using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Customer.Models.CustomerAccountEntry;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerAccountEntry)]
public class CustomerAccountEntryController : Controller
{
    private readonly ISession session;
    private readonly CustomerCodeService customerCodeService;

    public CustomerAccountEntryController(ISession session, CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/customer/account/entry")]
    public IActionResult Index()
    {
        var model = new CustomerAccountEntryIndexModel();
        model.ChangeTypeList = CustomerAccountChangeType.GetList();

        return View("CustomerAccountEntryIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/account/entry/list")]
    public IActionResult List(CustomerAccountEntryListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var accountEntryList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerAccountEntryInfo>();

        foreach (CustomerAccountEntryInfo accountEntry in accountEntryList)
        {
            accountEntry.CustomerCodeCache = customerCodeService.GetCustomerCode(accountEntry.Customer.Id);
        }

        var model = new CustomerAccountEntryListModel(listPager);
        model.CustomerAccountEntryList = accountEntryList;

        return View("CustomerAccountEntryList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerAccountEntryListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<CustomerAccountEntryInfo>();

        if (string.IsNullOrEmpty(queryParam.CustomerCodeQuery) == false)
        {
            var customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Where<CustomerAccountEntryInfo>(e => e.Customer.Id == customerIdQuery));
        }

        if (queryParam.ChangeTypeQuery != null && queryParam.ChangeTypeQuery != CustomerAccountChangeType.None)
        {
            var changeTypeQuery = queryParam.ChangeTypeQuery;
            criteria = criteria.Add(Restrictions.Where<CustomerAccountEntryInfo>(e => e.ChangeType == changeTypeQuery));
        }

        criteria = criteria.SetReadOnly(true);

        return criteria;
    }
}
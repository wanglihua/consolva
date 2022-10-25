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
    private ISession session;
    private CustomerCodeService customerCodeService;

    public CustomerAccountEntryController(ISession session, CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/customer/account/entry")]
    public IActionResult Index()
    {
        CustomerAccountEntryIndexModel model = new CustomerAccountEntryIndexModel();
        model.ChangeTypeList = CustomerAccountChangeType.GetList();

        return View("CustomerAccountEntryIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/account/entry/list")]
    public IActionResult List(CustomerAccountEntryListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<CustomerAccountEntryInfo> customerAccountEntryList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerAccountEntryInfo>();

        CustomerAccountEntryListModel model = new CustomerAccountEntryListModel(listPager);
        model.CustomerAccountEntryList = customerAccountEntryList;

        return View("CustomerAccountEntryList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerAccountEntryListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<CustomerAccountEntryInfo>();

        if (string.IsNullOrEmpty(queryParam.CustomerCodeQuery) == false)
        {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
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
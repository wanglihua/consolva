using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Customer.Models.CustomerAccountEntryCustomer;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerList)]
public class CustomerAccountEntryCustomerController : Controller {
    private readonly ISession session;
    private readonly CustomerCodeService customerCodeService;

    public CustomerAccountEntryCustomerController(ISession session, CustomerCodeService customerCodeService) {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/customer/account/entry/customer/{customerId:int}")]
    public IActionResult Index(int customerId) {
        CustomerInfo? customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            return Content("指定的客户Id不存在！");
        }

        customer.CustomerCodeCache = customerCodeService.GetCustomerCode(customer.Id);

        CustomerAccountEntryCustomerIndexModel model = new CustomerAccountEntryCustomerIndexModel();
        model.Customer = customer;

        return View("CustomerAccountEntryCustomerIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/account/entry/customer/list")]
    public IActionResult List(CustomerAccountEntryCustomerListQueryParam queryParam) {
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

        CustomerAccountEntryCustomerListModel model = new CustomerAccountEntryCustomerListModel(listPager);
        model.CustomerAccountEntryList = customerAccountEntryList;

        return View("CustomerAccountEntryCustomerList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerAccountEntryCustomerListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<CustomerAccountEntryInfo>();

        int customerId = queryParam.CustomerId;
        criteria = criteria.Add(Restrictions.Where<CustomerAccountEntryInfo>(e => e.Customer.Id == customerId));

        if (queryParam.ChangeTypeQuery != null && queryParam.ChangeTypeQuery != CustomerAccountChangeType.None) {
            var changeTypeQuery = queryParam.ChangeTypeQuery;
            criteria = criteria.Add(Restrictions.Where<CustomerAccountEntryInfo>(e => e.ChangeType == changeTypeQuery));
        }

        criteria = criteria.SetReadOnly(true);

        return criteria;
    }
}
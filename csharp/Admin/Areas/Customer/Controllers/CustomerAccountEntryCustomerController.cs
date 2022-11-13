using System.Collections.Generic;
using Core;
using Domain.Customer;
using Domain.Pass;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using Web.Areas.Admin.Models.CustomerAccountEntryCustomer;

namespace shunda.Admin.Areas.Customer.Controllers;

namespace Web.Areas.Admin.Controllers
{
    [AdminArea]
    [AdminAuth(PassRes.Customer.CustomerList)]
    public class CustomerAccountEntryCustomerController : Controller
    {
        private ISession session = null;

        public CustomerAccountEntryCustomerController(ISession session)
        {
            this.session = session;
        }

        [HttpGet]
        [Route("/admin/customer/account/entry/customer/{customerId:int}")]
        [NoHibernateAutoTransaction]
        public IActionResult Index(int customerId)
        {
            CustomerInfo customer = session.Get<CustomerInfo>(customerId);
            if (customer == null)
            {
                return Content("指定的客户Id不存在！");
            }

            CustomerAccountEntryCustomerIndexModel model = new CustomerAccountEntryCustomerIndexModel();
            model.Customer = customer;

            return View("CustomerAccountEntryCustomerIndex", model);
        }

        [HttpPost]
        [Route("/admin/customer/account/entry/customer/list")]
        [NoHibernateAutoTransaction]
        public IActionResult List(CustomerAccountEntryCustomerListQueryParam queryParam)
        {
            int totalCount = GetListCriteria(queryParam)
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();

            ListPager listPager = new ListPager(queryParam, totalCount);

            IList<CustomerAccountEntryInfo> customerAccountEntryList = GetListCriteria(queryParam)
                .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
                .AddOrder(Order.Desc(nameof(Model.Id)))
                .SetFirstResult(listPager.PageIndex * listPager.PageSize)
                .SetMaxResults(listPager.PageSize)
                .List<CustomerAccountEntryInfo>();

            CustomerAccountEntryCustomerListModel model = new CustomerAccountEntryCustomerListModel(listPager);
            model.CustomerAccountEntryList = customerAccountEntryList;

            return View("CustomerAccountEntryCustomerList", model);
        }

        [NonAction]
        private ICriteria GetListCriteria(CustomerAccountEntryCustomerListQueryParam queryParam)
        {
            ICriteria criteria = session.CreateCriteria<CustomerAccountEntryInfo>();

            int customerId = queryParam.CustomerId;
            criteria = criteria.Add(Restrictions.Where<CustomerAccountEntryInfo>(e => e.Customer.Id == customerId));

            if (queryParam.ChangeTypeQuery != null && queryParam.ChangeTypeQuery != CustomerAccountChangeType.None)
            {
                var changeTypeQuery = queryParam.ChangeTypeQuery;
                criteria = criteria.Add(Restrictions.Where<CustomerAccountEntryInfo>(e => e.ChangeType == changeTypeQuery));
            }

            criteria = criteria.SetReadOnly(true);

            return criteria;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Core;
using Domain.Customer;
using Domain.LogisticsOrder;
using Domain.Pass;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using Web.Areas.Admin.Models.CustomerScoreLogCustomer;

namespace Web.Areas.Admin.Controllers
{
    [AdminArea]
    [AdminAuth(PassRes.Customer.CustomerList)]
    public class CustomerScoreLogCustomerController : Controller
    {
        private ISession session = null;

        public CustomerScoreLogCustomerController(ISession session)
        {
            this.session = session;
        }

        [HttpGet]
        [Route("/admin/customer/score/log/{customerId:int}")]
        [NoHibernateAutoTransaction]
        public IActionResult Index(int customerId)
        {
            CustomerInfo customer = session.Get<CustomerInfo>(customerId);
            if (customer == null)
            {
                throw new ValidationException("指定的会员不存在！");
            }

            CustomerScoreLogCustomerIndexModel model = new CustomerScoreLogCustomerIndexModel();
            model.CustomerId = customerId;
            model.Customer = customer;
            model.ScoreTypeList = CustomerScoreType.GetList();

            return View("CustomerScoreLogCustomerIndex", model);
        }

        [HttpPost]
        [Route("/admin/customer/score/log/customer/list")]
        [NoHibernateAutoTransaction]
        public IActionResult List(CustomerScoreLogCustomerListQueryParam queryParam)
        {
            int totalCount = GetListCriteria(queryParam)
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();

            ListPager listPager = new ListPager(queryParam, totalCount);

            IList<CustomerScoreLogInfo> customerScoreLogList = GetListCriteria(queryParam)
                .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
                .AddOrder(Order.Desc(nameof(Model.Id)))
                .SetFirstResult(listPager.PageIndex * listPager.PageSize)
                .SetMaxResults(listPager.PageSize)
                .List<CustomerScoreLogInfo>();

            foreach (CustomerScoreLogInfo customerScoreLogInfo in customerScoreLogList)
            {
                // customerScoreLogInfo
            }

            CustomerScoreLogCustomerListModel model = new CustomerScoreLogCustomerListModel(listPager);
            model.CustomerScoreLogList = customerScoreLogList;

            return View("CustomerScoreLogCustomerList", model);
        }

        [NonAction]
        private ICriteria GetListCriteria(CustomerScoreLogCustomerListQueryParam queryParam)
        {
            ICriteria criteria = session.CreateCriteria<CustomerScoreLogInfo>();
            criteria = criteria.SetReadOnly(true);

            int customerId = queryParam.CustomerId;
            criteria = criteria.Add(Restrictions.Where<CustomerScoreLogInfo>(e => e.Customer.Id == customerId));

            if (queryParam.ScoreTypeQuery != null && queryParam.ScoreTypeQuery != CustomerScoreType.None)
            {
                criteria = criteria.Add(Restrictions.Where<CustomerScoreLogInfo>(e => e.ScoreType == queryParam.ScoreTypeQuery));
            }

            if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
            {
                string orderCode = queryParam.OrderCodeQuery.Trim();
                LogisticsOrderInfo logisticsOrder = session.Query<LogisticsOrderInfo>()
                    .Where(e => e.Code == orderCode)
                    .Take(1)
                    .SingleOrDefault();

                int orderId = logisticsOrder?.Id ?? 0;
                criteria = criteria.Add(Restrictions.Where<CustomerScoreLogInfo>(e => e.LogisticsOrder.Id == orderId));
            }

            return criteria;
        }
    }
}
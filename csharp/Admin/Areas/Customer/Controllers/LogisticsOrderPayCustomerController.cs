using System.Linq;
using Core;
using Domain.Customer;
using Domain.LogisticsOrder;
using Domain.Pass;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using Web.Areas.Admin.Models.LogisticsOrderPayCustomer;

namespace Web.Areas.Admin.Controllers
{
    [AdminArea]
    [AdminAuth(PassRes.Customer.CustomerList)] // 这个一般是在客户列表里面打开查看的
    public class LogisticsOrderPayCustomerController : Controller
    {
        private readonly ISession session;

        public LogisticsOrderPayCustomerController(ISession session)
        {
            this.session = session;
        }

        [HttpGet]
        [Route("/admin/logistics/order/pay/{customerId:int}")]
        [NoHibernateAutoTransaction]
        public IActionResult Index(int customerId)
        {
            var customer = session.Get<CustomerInfo>(customerId);
            if (customer == null) throw new ValidationException("指定的会员不存在！");

            var model = new LogisticsOrderPayCustomerIndexModel();
            model.CustomerId = customerId;
            model.Customer = customer;

            return View("LogisticsOrderPayCustomerIndex", model);
        }

        [HttpPost]
        [Route("/admin/logistics/order/pay/customer/list")]
        [NoHibernateAutoTransaction]
        public IActionResult List(LogisticsOrderPayCustomerListQueryParam queryParam)
        {
            var totalCount = GetListCriteria(queryParam)
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();

            var listPager = new ListPager(queryParam, totalCount);

            var logisticsOrderPayList = GetListCriteria(queryParam)
                .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
                .AddOrder(Order.Desc(nameof(Model.Id)))
                .SetFirstResult(listPager.PageIndex * listPager.PageSize)
                .SetMaxResults(listPager.PageSize)
                .List<LogisticsOrderPayInfo>();

            foreach (var logisticsOrderPayInfo in logisticsOrderPayList)
            {
                // logisticsOrderPayInfo
            }

            var model = new LogisticsOrderPayCustomerListModel(listPager);
            model.LogisticsOrderPayList = logisticsOrderPayList;

            return View("LogisticsOrderPayCustomerList", model);
        }

        [NonAction]
        private ICriteria GetListCriteria(LogisticsOrderPayCustomerListQueryParam queryParam)
        {
            var criteria = session.CreateCriteria<LogisticsOrderPayInfo>();
            criteria = criteria.CreateAlias(nameof(LogisticsOrderPayInfo.Customer), nameof(LogisticsOrderPayInfo.Customer));
            criteria = criteria.SetReadOnly(true);

            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.Status == LogisticsOrderPayStatus.Paid));

            // 排除掉费用核销的费用
            // 可能也需要的，比如线下付款的 criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.PayType != PayType.WriteOff));

            // 只显示指定会员的
            var customerId = queryParam.CustomerId;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.Customer.Id == customerId));

            var orderCodeQuery = queryParam.OrderCodeQuery;
            if (string.IsNullOrWhiteSpace(orderCodeQuery) == false)
            {
                var logisticsOrder = session.Query<LogisticsOrderInfo>()
                    .Where(e => e.Code == orderCodeQuery)
                    .Take(1)
                    .SingleOrDefault();

                var orderId = logisticsOrder?.Id ?? 0;
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.Order.Id == orderId));
            }

            return criteria;
        }
    }
}
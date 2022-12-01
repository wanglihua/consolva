using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Customer.Models.LogisticsOrderPayCustomer;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerList)] // 这个一般是在客户列表里面打开查看的
public class LogisticsOrderPayCustomerController : Controller
{
    private readonly ISession session;
    private CustomerCodeService customerCodeService;

    public LogisticsOrderPayCustomerController(ISession session, CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/logistics/order/pay/{customerId:int}")]
    public IActionResult Index(int customerId)
    {
        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null)
        {
            throw new ValidationException("指定的会员不存在！");
        }

        customer.CustomerCodeCache = customerCodeService.GetCustomerCode(customer.Id);

        var model = new LogisticsOrderPayCustomerIndexModel();
        model.CustomerId = customerId;
        model.Customer = customer;

        return View("LogisticsOrderPayCustomerIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/pay/customer/list")]
    public IActionResult List(LogisticsOrderPayCustomerListQueryParam queryParam)
    {
        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            var orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            int orderId = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCodeQuery)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();

            queryParam.OrderId = orderId;
        }

        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var logisticsOrderPayList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsOrderPayInfo>();

        foreach (LogisticsOrderPayInfo orderPay in logisticsOrderPayList)
        {
            orderPay.CustomerCodeCache = customerCodeService.GetCustomerCode(orderPay.Customer.Id);
        }

        var model = new LogisticsOrderPayCustomerListModel(listPager);
        model.LogisticsOrderPayList = logisticsOrderPayList;

        return View("LogisticsOrderPayCustomerList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(LogisticsOrderPayCustomerListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<LogisticsOrderPayInfo>();
        // criteria = criteria.CreateAlias(nameof(LogisticsOrderPayInfo.Customer), nameof(LogisticsOrderPayInfo.Customer));
        criteria = criteria.CreateAlias(nameof(LogisticsOrderPayInfo.Order), nameof(LogisticsOrderPayInfo.Order));
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.Status == PayStatus.Paid));

        // 排除掉费用核销的费用
        // 可能也需要的，比如线下付款的 criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.PayType != PayType.WriteOff));

        // 只显示指定会员的
        var customerId = queryParam.CustomerId;
        criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.Customer.Id == customerId));

        if (queryParam.OrderId != null)
        {
            int orderId = queryParam.OrderId.Value;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.Order.Id == orderId));
        }

        return criteria;
    }
}
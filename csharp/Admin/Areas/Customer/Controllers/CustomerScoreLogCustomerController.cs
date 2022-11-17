using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Customer.Models.CustomerScoreLogCustomer;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Order;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerList)]
public class CustomerScoreLogCustomerController : Controller
{
    private ISession session;
    private CustomerCodeService customerCodeService;

    public CustomerScoreLogCustomerController(ISession session, CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/customer/score/log/{customerId:int}")]
    public IActionResult Index(int customerId)
    {
        CustomerInfo? customer = session.Get<CustomerInfo>(customerId);
        if (customer == null)
        {
            throw new ValidationException("指定的会员不存在！");
        }

        customer.CustomerCodeCache = customerCodeService.GetCustomerCode(customer.Id);

        CustomerScoreLogCustomerIndexModel model = new CustomerScoreLogCustomerIndexModel();
        model.CustomerId = customerId;
        model.Customer = customer;
        model.ScoreTypeList = CustomerScoreType.GetList();

        return View("CustomerScoreLogCustomerIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/score/log/customer/list")]
    public IActionResult List(CustomerScoreLogCustomerListQueryParam queryParam)
    {
        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            string orderCode = queryParam.OrderCodeQuery.Trim();
            int orderId = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCode)
                .Select(e =>e.Id)
                .Take(1)
                .SingleOrDefault();

            queryParam.OrderId = orderId;
        }
        
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<CustomerScoreLogInfo> customerScoreLogList = GetListCriteria(queryParam)
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

        if (queryParam.OrderId != 0)
        {
            int orderId = queryParam.OrderId;
            criteria = criteria.Add(Restrictions.Where<CustomerScoreLogInfo>(e => e.LogisticsOrder!.Id == orderId));
        }

        return criteria;
    }
}
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Customer.Models.CustomerScoreLog;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Order;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerScoreLog)]
public class CustomerScoreLogController : Controller
{
    private ISession session;
    private CustomerCodeService customerCodeService;

    public CustomerScoreLogController(ISession session, CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/customer/score/log")]
    public IActionResult Index()
    {
        CustomerScoreLogIndexModel model = new CustomerScoreLogIndexModel();
        model.ScoreTypeList = CustomerScoreType.GetList();

        return View("CustomerScoreLogIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/score/log/list")]
    public IActionResult List(CustomerScoreLogListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<CustomerScoreLogInfo> customerScoreLogList = GetListCriteria(queryParam)
            .AddOrder(new NHibernate.Criterion.Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(NHibernate.Criterion.Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerScoreLogInfo>();

        CustomerScoreLogListModel model = new CustomerScoreLogListModel(listPager);
        model.CustomerScoreLogList = customerScoreLogList;

        return View("CustomerScoreLogList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerScoreLogListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<CustomerScoreLogInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Where<CustomerScoreLogInfo>(e => e.Customer.Id == customerIdQuery));
        }

        if (queryParam.ScoreTypeQuery != null && queryParam.ScoreTypeQuery != CustomerScoreType.None)
        {
            criteria = criteria.Add(Restrictions.Where<CustomerScoreLogInfo>(e => e.ScoreType == queryParam.ScoreTypeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            string orderCode = queryParam.OrderCodeQuery.Trim();
            LogisticsOrderInfo? logisticsOrder = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCode)
                .Take(1)
                .SingleOrDefault();

            int orderId = logisticsOrder?.Id ?? 0;
            criteria = criteria.Add(Restrictions.Where<CustomerScoreLogInfo>(e =>  e.LogisticsOrder!.Id == orderId));
        }

        return criteria;
    }
}
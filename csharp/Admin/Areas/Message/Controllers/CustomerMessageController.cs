using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Message.Models.CustomerMessage;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Message;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Message.Controllers;

[MessageArea]
[AdminAuth(PassRes.CustomerMessage.MessageList)]
public class CustomerMessageController : Controller
{
    private ISession session;
    private CustomerCodeService customerCodeService;

    public CustomerMessageController(ISession session, CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/customer/message")]
    public IActionResult Index()
    {
        CustomerMessageIndexModel model = new CustomerMessageIndexModel();
        model.MessageTypeList = CustomerMessageType.GetList();
        model.SendTypeList = CustomerMessageSendType.GetList();

        return View("CustomerMessageIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/message/list")]
    public IActionResult List(CustomerMessageListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<CustomerMessageInfo> customerMessageList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerMessageInfo>();

        foreach (CustomerMessageInfo message in customerMessageList)
        {
            message.CustomerCodeCache = customerCodeService.GetCustomerCode(message.Customer.Id);
        }

        CustomerMessageListModel model = new CustomerMessageListModel(listPager);
        model.CustomerMessageList = customerMessageList;

        return View("CustomerMessageList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerMessageListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<CustomerMessageInfo>();

        criteria = criteria.SetReadOnly(true);

        if (queryParam.MessageTypeQuery != null && queryParam.MessageTypeQuery != CustomerMessageType.None)
        {
            criteria = criteria.Add(Restrictions.Where<CustomerMessageInfo>(e => e.Type == queryParam.MessageTypeQuery));
        }

        if (queryParam.SendTypeQuery != null && queryParam.SendTypeQuery != CustomerMessageSendType.None)
        {
            criteria = criteria.Add(Restrictions.Where<CustomerMessageInfo>(e => e.SendType == queryParam.SendTypeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Where<CustomerMessageInfo>(e => e.Customer.Id == customerIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.ContentQuery) == false)
        {
            string contentQuery = queryParam.ContentQuery.Trim();
            criteria = criteria.Add(Restrictions.On<CustomerMessageInfo>(e => e.Content).IsInsensitiveLike(contentQuery, MatchMode.Anywhere));
        }

        if (queryParam.SendBySmsQuery != null)
        {
            bool sendBySmsQueryValue = queryParam.SendBySmsQuery.Value;
            criteria = criteria.Add(Restrictions.Where<CustomerMessageInfo>(e => e.SmsSend == sendBySmsQueryValue));
        }

        if (queryParam.SendByWechatQuery != null)
        {
            bool sendByWechatQueryValue = queryParam.SendByWechatQuery.Value;
            criteria = criteria.Add(Restrictions.Where<CustomerMessageInfo>(e => e.WechatSend == sendByWechatQueryValue));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            var order = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCodeQuery)
                .Take(1)
                .FirstOrDefault();
            if (order != null)
            {
                int orderId = order.Id;
                criteria = criteria.Add(Restrictions.Eq($"{nameof(CustomerMessageInfo.Order)}.{nameof(LogisticsOrderInfo.Id)}", orderId));
            }
        }

        return criteria;
    }
}
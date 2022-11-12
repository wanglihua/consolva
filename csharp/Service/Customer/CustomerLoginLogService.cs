using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using NLog;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Service.Wechat;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerLoginLogService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;
    private WebSessionService webSessionService;
    private WechatSessionService wechatSessionService;

    public CustomerLoginLogService(
        ISession session,
        WebSessionService webSessionService,
        WechatSessionService wechatSessionService
    )
    {
        this.session = session;
        this.webSessionService = webSessionService;
        this.wechatSessionService = wechatSessionService;
    }

    private void AddLogInternal(CustomerLoginStage stage, int customerId, HttpRequest request)
    {
        if (stage == null)
        {
            stage = CustomerLoginStage.None;
        }

        CustomerInfo customer = session.Get<CustomerInfo>(customerId);
        if (customer == null)
        {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }


        string sessionId = webSessionService.SessionId;
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            sessionId = string.Empty;
        }

        if (string.IsNullOrWhiteSpace(openId))
        {
            openId = string.Empty;
        }

        CustomerLoginLogInfo customerLoginLog = new CustomerLoginLogInfo();
        customerLoginLog.Stage = stage;
        customerLoginLog.Customer = customer;
        customerLoginLog.OpenId = openId;
        customerLoginLog.SessionId = sessionId;
        customerLoginLog.IpAddress = StringUtil.LimitMaxLength(WebUtil.GetClientIp(request), 180); // not null
        customerLoginLog.UserAgent = StringUtil.LimitMaxLength(WebUtil.GetUserAgent(request), 280); // not null
        customerLoginLog.LoginTime = DateTime.Now;

        session.Save(customerLoginLog);
        session.Flush();
    }
}
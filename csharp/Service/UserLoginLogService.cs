using Microsoft.AspNetCore.Http;
using NLog;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Domain.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Service;

[Service(ServiceType.Scoped)]
public class UserLoginLogService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;

    public UserLoginLogService(ISession session)
    {
        this.session = session;
    }

    public void AddLog(UserInfo user, HttpRequest request)
    {
        try
        {
            UserLoginLogInfo userLoginLog = new UserLoginLogInfo();
            userLoginLog.User = user;
            userLoginLog.IpAddress = StringUtil.LimitMaxLength(WebUtil.GetClientIp(request), 180); // not null
            userLoginLog.UserAgent = StringUtil.LimitMaxLength(WebUtil.GetUserAgent(request), 280); // not null
            userLoginLog.LoginTime = DateTime.Now;

            session.Save(userLoginLog);
            session.Flush();
        }
        catch (Exception exp)
        {
            log.Error(exp);
        }
    }
}
using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service;

/// <summary>
/// 用户 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class UserService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;

    public UserService(ISession session)
    {
        this.session = session;
    }
}
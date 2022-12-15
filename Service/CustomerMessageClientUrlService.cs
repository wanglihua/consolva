using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service;

/// <summary>
/// 客户消息客户端URL 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageClientUrlService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public CustomerMessageClientUrlService(ISession session)
    {
        this.session = session;
    }
}
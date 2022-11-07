using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service;

/// <summary>
/// 快递单号预报 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class ExpressExpectService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;

    public ExpressExpectService(ISession session)
    {
        this.session = session;
    }
}
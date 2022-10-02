using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service;

/// <summary>
/// 物流方式打包方式 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class LogisticsModeRepackTypeService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;

    public LogisticsModeRepackTypeService(ISession session)
    {
        this.session = session;
    }
}
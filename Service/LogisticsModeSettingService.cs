using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service;

/// <summary>
/// 物流配置 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class LogisticsModeSettingService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public LogisticsModeSettingService(ISession session) {
        this.session = session;
    }
}
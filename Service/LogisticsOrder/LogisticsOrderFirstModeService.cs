using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service.LogisticsOrder;

/// <summary>
/// 物流订单头程 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class LogisticsOrderFirstModeService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public LogisticsOrderFirstModeService(ISession session) {
        this.session = session;
    }
}
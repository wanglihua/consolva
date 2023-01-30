using NHibernate;
using NLog;
using shunda.Core.Service;

namespace shunda.Service.Logistics;

/// <summary>
/// 物流方式通用费率 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class LogisticsModeFeeRateService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public LogisticsModeFeeRateService(ISession session) {
        this.session = session;
    }
}
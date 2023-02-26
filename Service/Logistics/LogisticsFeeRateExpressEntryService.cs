using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service.Logistics;

/// <summary>
/// 快递具体费率明细 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class LogisticsFeeRateExpressEntryService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public LogisticsFeeRateExpressEntryService(ISession session) {
        this.session = session;
    }
}
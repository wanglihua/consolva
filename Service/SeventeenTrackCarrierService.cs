using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service;

/// <summary>
/// 17track运输商 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class SeventeenTrackCarrierService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public SeventeenTrackCarrierService(ISession session) {
        this.session = session;
    }
}
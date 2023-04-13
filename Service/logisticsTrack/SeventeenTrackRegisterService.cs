using NLog;
using shunda.Core.Service;
using ISession = NHibernate.ISession;

namespace shunda.Service.logisticsTrack;

/// <summary>
/// 17track单号注册
/// </summary>
[Service(ServiceType.Scoped)]
public class SeventeenTrackRegisterService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public SeventeenTrackRegisterService(ISession session) {
        this.session = session;
    }
}
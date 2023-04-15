using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service.logisticsTrack;

/// <summary>
/// 17TRACK注册单号 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class SeventeenTrackNumberService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public SeventeenTrackNumberService(ISession session) {
        this.session = session;
    }
}
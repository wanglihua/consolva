using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service.logisticsTrack;

/// <summary>
/// 17TRACK快递单事件 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class SeventeenTrackNumberEventService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public SeventeenTrackNumberEventService(ISession session) {
        this.session = session;
    }
}
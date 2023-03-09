using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service;

/// <summary>
/// 广告横幅 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class AdvertBannerService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public AdvertBannerService(ISession session) {
        this.session = session;
    }
}
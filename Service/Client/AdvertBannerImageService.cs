using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service.Client;

/// <summary>
/// 广告横幅图片 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class AdvertBannerImageService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public AdvertBannerImageService(ISession session) {
        this.session = session;
    }
}
using NHibernate;
using NLog;
using shunda.Core.Service;
using shunda.Domain.Client;

namespace shunda.Service.Client;

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

    /// <summary>
    /// 创建广告位
    /// </summary>
    public void CreateBannersIfNotExist() {
        IList<AdvertBannerInfo> advertBannerList = session.Query<AdvertBannerInfo>().ToList();
        IList<AdvertBannerLoc> advertBannerLocList = AdvertBannerLoc.GetList();
        foreach (AdvertBannerLoc advertBannerLoc in advertBannerLocList) {
            var advertBanner = advertBannerList
                .Where(e => e.Loc.Id == advertBannerLoc.Id)
                .Take(1)
                .SingleOrDefault();

            if (advertBanner == null) {
                advertBanner = new AdvertBannerInfo {
                    Loc = advertBannerLoc,
                    Name = advertBannerLoc.Description + "广告位",
                    ClientShow = false
                };

                session.Save(advertBanner);
            }
        }

        session.Flush();
    }
}
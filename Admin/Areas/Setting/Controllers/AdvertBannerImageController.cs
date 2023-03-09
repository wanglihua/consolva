using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Client;
using shunda.Admin.Areas.Setting.Models.AdvertBannerImage;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Setting.Controllers;

/// <summary>
/// 广告横幅图片 控制器
/// </summary>
[SettingArea]
[AdminAuth(PassRes.Setting.AdvertBanner)]
public class AdvertBannerImageController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public AdvertBannerImageController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/setting/advert/banner/image/{bannerId:int}")]
    public IActionResult Index(int bannerId) {
        AdvertBannerInfo advertBanner = session.Get<AdvertBannerInfo>(bannerId);
        if (advertBanner == null) {
            throw new ValidationException("指定的广告横幅不存在！");
        }

        AdvertBannerImageIndexModel model = new AdvertBannerImageIndexModel {
            AdvertBanner = advertBanner
        };

        return View("AdvertBannerImageIndex", model);
    }

    [HttpPost]
    [Route("/admin/setting/advert/banner/image/list")]
    public IActionResult List(AdvertBannerImageListQueryParam queryParam) {
        var listQuery = GetQueryOver(queryParam);

        listQuery = listQuery
            .OrderByAlias(() => advertBannerImageAlias!.ImageNum).Asc
            .ThenByAlias(() => advertBannerImageAlias!.Id).Asc;

        IList<AdvertBannerImageInfo> advertBannerImageList = listQuery
            .List<AdvertBannerImageInfo>();

        AdvertBannerImageListModel model = new AdvertBannerImageListModel {
            AdvertBannerImageList = advertBannerImageList,
            AdvertBannerImageAlias = nameof(advertBannerImageAlias),
            BannerAlias = nameof(bannerAlias),
        };

        return View("AdvertBannerImageList", model);
    }

    private AdvertBannerImageInfo? advertBannerImageAlias = null;
    private AdvertBannerInfo? bannerAlias = null;

    [NonAction]
    private IQueryOver<AdvertBannerImageInfo, AdvertBannerImageInfo> GetQueryOver(AdvertBannerImageListQueryParam queryParam) {
        var query = session.QueryOver<AdvertBannerImageInfo>(() => advertBannerImageAlias!)
            .JoinAlias(() => advertBannerImageAlias!.Banner, () => bannerAlias);

        int advertBannerId = queryParam.AdvertBannerId;
        query = query.Where(e => e.Banner.Id == advertBannerId);

        return query;
    }

    [HttpPost]
    [Route("/admin/setting/advert/banner/image/detail")]
    public IActionResult Detail(IdParam param) {
        var id = param.Id;

        AdvertBannerImageInfo? advertBannerImage;

        if (id <= 0) {
            advertBannerImage = new AdvertBannerImageInfo();
        } else {
            advertBannerImage = session.Get<AdvertBannerImageInfo>(id);
            if (advertBannerImage == null) {
                throw new ValidationException($"指定的 广告横幅图片#{id} 不存在！");
            }
        }

        AdvertBannerImageDetailModel model = new AdvertBannerImageDetailModel();
        model.AdvertBannerImageId = advertBannerImage.Id;

        model.BannerId = advertBannerImage.Banner.Id;
        model.ImageNum = advertBannerImage.ImageNum;
        model.StorePath = advertBannerImage.StorePath;
        model.StoreName = advertBannerImage.StoreName;
        model.LinkUrl = advertBannerImage.LinkUrl;

        return View("AdvertBannerImageDetail", model);
    }

    [HttpPost]
    [Route("/admin/setting/advert/banner/image/save")]
    [Transactional]
    public IActionResult Save(AdvertBannerImageSaveModel model) {
        ValidationUtil.Validate<AdvertBannerImageSaveModelValidator, AdvertBannerImageSaveModel>(model);

        int advertBannerImageId = model.AdvertBannerImageId;

        bool isCreate = advertBannerImageId <= 0;

        AdvertBannerImageInfo advertBannerImage;

        if (isCreate) {
            advertBannerImage = new AdvertBannerImageInfo();

            var banner = session.Get<AdvertBannerInfo>(model.BannerId);
            if (banner == null) {
                throw new ValidationException($"指定的广告横幅#{model.BannerId}不存在！");
            }

            advertBannerImage.Banner = banner;
        } else {
            advertBannerImage = session.Get<AdvertBannerImageInfo>(advertBannerImageId);
            if (advertBannerImage == null) {
                throw new ValidationException($"指定的 广告横幅图片#{advertBannerImageId} 不存在！");
            }
        }

        advertBannerImage.ImageNum = model.ImageNum;
        advertBannerImage.StorePath = model.StorePath;
        advertBannerImage.StoreName = model.StoreName;
        advertBannerImage.LinkUrl = model.LinkUrl;

        if (isCreate) {
            session.Save(advertBannerImage);
        } else {
            session.Update(advertBannerImage);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
        });
    }

    [HttpPost]
    [Route("/admin/setting/advert/banner/image/delete")]
    [Transactional]
    public IActionResult Delete(IdListParam param) {
        param.Validate("请选择要删除的广告横幅图片！");

        var idList = param.IdList;

        foreach (int id in idList) {
            AdvertBannerImageInfo advertBannerImage = session.Load<AdvertBannerImageInfo>(id);
            session.Delete(advertBannerImage);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "删除成功！",
        });
    }
}
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NLog;
using shunda.Admin.Areas.Setting.Models.AdvertBanner;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Client;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Client;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Setting.Controllers;

/// <summary>
/// 广告横幅 控制器
/// </summary>
[SettingArea]
[AdminAuth(PassRes.Setting.AdvertBanner)]
public class AdvertBannerController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly AdvertBannerService advertBannerService;

    public AdvertBannerController(ISession session, AdvertBannerService advertBannerService) {
        this.session = session;
        this.advertBannerService = advertBannerService;
    }

    [HttpGet]
    [Route("/admin/setting/advert/banner")]
    [Transactional]
    public IActionResult Index() {
        AdvertBannerIndexModel model = new AdvertBannerIndexModel {
            LocList = AdvertBannerLoc.GetList()
        };

        advertBannerService.CreateBannersIfNotExist();

        return View("AdvertBannerIndex", model);
    }

    [HttpPost]
    [Route("/admin/setting/advert/banner/list")]
    public IActionResult List(AdvertBannerListQueryParam queryParam) {
        var listQuery = GetQueryOver(queryParam);
        listQuery = listQuery.ThenByAlias(() => advertBannerAlias!.Id).Asc;

        IList<AdvertBannerInfo> advertBannerList = listQuery
            .List<AdvertBannerInfo>();

        AdvertBannerListModel model = new AdvertBannerListModel {
            AdvertBannerList = advertBannerList,
            AdvertBannerAlias = nameof(advertBannerAlias)
        };

        return View("AdvertBannerList", model);
    }

    private AdvertBannerInfo? advertBannerAlias = null;

    [NonAction]
    private IQueryOver<AdvertBannerInfo, AdvertBannerInfo> GetQueryOver(AdvertBannerListQueryParam queryParam) {
        var query = session.QueryOver<AdvertBannerInfo>(() => advertBannerAlias!);

        return query;
    }

    [HttpPost]
    [Route("/admin/setting/advert/banner/detail")]
    public IActionResult Detail(IdParam param) {
        var id = param.Id;

        AdvertBannerInfo? advertBanner;

        if (id <= 0) {
            advertBanner = new AdvertBannerInfo();
        } else {
            advertBanner = session.Get<AdvertBannerInfo>(id);
            if (advertBanner == null) {
                throw new ValidationException($"指定的 广告横幅#{id} 不存在！");
            }
        }

        AdvertBannerDetailModel model = new AdvertBannerDetailModel {
            AdvertBannerId = advertBanner.Id,
            Loc = advertBanner.Loc,
            Name = advertBanner.Name,
            ClientShow = advertBanner.ClientShow,
            LocList = AdvertBannerLoc.GetList()
        };

        return View("AdvertBannerDetail", model);
    }

    [HttpPost]
    [Route("/admin/setting/advert/banner/save")]
    [Transactional]
    public IActionResult Save(AdvertBannerSaveModel model) {
        ValidationUtil.Validate<AdvertBannerSaveModelValidator, AdvertBannerSaveModel>(model);

        int advertBannerId = model.AdvertBannerId;

        bool isCreate = advertBannerId <= 0;

        AdvertBannerInfo advertBanner;

        if (isCreate) {
            advertBanner = new AdvertBannerInfo();
        } else {
            advertBanner = session.Get<AdvertBannerInfo>(advertBannerId);
            if (advertBanner == null) {
                throw new ValidationException($"指定的 广告横幅#{advertBannerId} 不存在！");
            }
        }

        advertBanner.Loc = model.Loc;
        advertBanner.Name = model.Name;
        advertBanner.ClientShow = model.ClientShow;

        if (isCreate) {
            session.Save(advertBanner);
        } else {
            session.Update(advertBanner);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
        });
    }
}
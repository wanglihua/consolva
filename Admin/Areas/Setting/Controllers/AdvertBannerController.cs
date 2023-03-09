using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Client;
using shunda.Admin.Areas.Setting.Models.AdvertBanner;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using shunda.Domain.Pass.PassRes;
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

    public AdvertBannerController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/setting/advert/banner")]
    public IActionResult Index() {
        AdvertBannerIndexModel model = new AdvertBannerIndexModel();

        model.LocList = AdvertBannerLoc.GetList();

        return View("AdvertBannerIndex", model);
    }

    [HttpPost]
    [Route("/admin/setting/advert/banner/list")]
    public IActionResult List(AdvertBannerListQueryParam queryParam) {
        int totalCount = GetQueryOver(queryParam)
            // .Select(Projections.Count<AdvertBannerInfo>(e => advertBannerAlias!.Id))
            .Select(Projections.RowCount())
            .SingleOrDefault<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        var listQuery = GetQueryOver(queryParam);
        if (listPager.IsAscending) {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Asc;
        } else {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Desc;
        }

        listQuery = listQuery.ThenByAlias(() => advertBannerAlias!.Id).Desc;
            
        IList<AdvertBannerInfo> advertBannerList = listQuery
            .Skip(listPager.Skip())
            .Take(listPager.Take())
            .List<AdvertBannerInfo>();

        /*
        foreach (AdvertBannerInfo advertBanner in advertBannerList) {
        }
        */

        AdvertBannerListModel model = new AdvertBannerListModel(listPager);
        model.AdvertBannerList = advertBannerList;
        
        model.AdvertBannerAlias = nameof(advertBannerAlias);        

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

        AdvertBannerDetailModel model = new AdvertBannerDetailModel();
        model.AdvertBannerId = advertBanner.Id;

        model.Loc = advertBanner.Loc;
        model.Name = advertBanner.Name;
        model.ClientShow = advertBanner.ClientShow;

        model.LocList = AdvertBannerLoc.GetList();

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
            if (advertBanner == null){
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

    [HttpPost]
    [Route("/admin/setting/advert/banner/delete")]
    [Transactional]
    public IActionResult Delete(IdListParam param) {
        param.Validate("请选择要删除的广告横幅！");

        var idList = param.IdList;

        foreach (int id in idList) {
            AdvertBannerInfo advertBanner = session.Load<AdvertBannerInfo>(id);
            session.Delete(advertBanner);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "删除成功！",
        });
    }
}
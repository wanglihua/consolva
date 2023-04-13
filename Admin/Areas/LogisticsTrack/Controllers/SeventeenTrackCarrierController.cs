using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackCarrier;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.LogisticsTrack;
using shunda.Domain.Pass.PassRes;
using shunda.Service.logisticsTrack.Carrier;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsTrack.Controllers;

/// <summary>
/// 17track运输商 控制器
/// </summary>
[LogisticsTrackArea]
[AdminAuth(PassRes.LogisticsTrack.SeventeenTrackCarrier)]
public class SeventeenTrackCarrierController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly SeventeenTrackCarrierService seventeenTrackCarrierService;

    public SeventeenTrackCarrierController(
        ISession session,
        SeventeenTrackCarrierService seventeenTrackCarrierService
    ) {
        this.session = session;
        this.seventeenTrackCarrierService = seventeenTrackCarrierService;
    }

    [HttpGet]
    [Route("/admin/seventeen/track/carrier")]
    public IActionResult Index() {
        SeventeenTrackCarrierIndexModel model = new SeventeenTrackCarrierIndexModel();

        return View("SeventeenTrackCarrierIndex", model);
    }

    [HttpPost]
    [Route("/admin/seventeen/track/carrier/list")]
    public IActionResult List(SeventeenTrackCarrierListQueryParam queryParam) {
        int totalCount = GetQueryOver(queryParam)
            // .Select(Projections.Count<SeventeenTrackCarrierInfo>(e => seventeenTrackCarrierAlias!.Id))
            .Select(Projections.RowCount())
            .SingleOrDefault<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        var listQuery = GetQueryOver(queryParam);
        if (listPager.IsAscending) {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Asc;
        } else {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Desc;
        }

        listQuery = listQuery.ThenByAlias(() => seventeenTrackCarrierAlias!.Id).Desc;

        IList<SeventeenTrackCarrierInfo> seventeenTrackCarrierList = listQuery
            .Skip(listPager.Skip())
            .Take(listPager.Take())
            .List<SeventeenTrackCarrierInfo>();

        /*
        foreach (SeventeenTrackCarrierInfo seventeenTrackCarrier in seventeenTrackCarrierList) {
        }
        */

        SeventeenTrackCarrierListModel model = new SeventeenTrackCarrierListModel(listPager);
        model.SeventeenTrackCarrierList = seventeenTrackCarrierList;

        model.SeventeenTrackCarrierAlias = nameof(seventeenTrackCarrierAlias);

        return View("SeventeenTrackCarrierList", model);
    }

    private SeventeenTrackCarrierInfo? seventeenTrackCarrierAlias = null;

    [NonAction]
    private IQueryOver<SeventeenTrackCarrierInfo, SeventeenTrackCarrierInfo> GetQueryOver(SeventeenTrackCarrierListQueryParam queryParam) {
        var query = session.QueryOver<SeventeenTrackCarrierInfo>(() => seventeenTrackCarrierAlias!);

        if (string.IsNullOrWhiteSpace(queryParam.CarrierKeyQuery) == false) {
            string carrierKeyQuery = queryParam.CarrierKeyQuery.Trim();
            int.TryParse(carrierKeyQuery, out int carrierKeyQueryInt);

            query = query.Where(e => e.CarrierKey == carrierKeyQueryInt);
        }

        if (string.IsNullOrWhiteSpace(queryParam.CarrierNameQuery) == false) {
            string carrierNameQuery = queryParam.CarrierNameQuery.Trim();
            query = query.Where(e => e.CarrierName.IsInsensitiveLike(carrierNameQuery, MatchMode.Anywhere)
                                     || e.NameZhCn.IsInsensitiveLike(carrierNameQuery, MatchMode.Anywhere)
                                     || e.NameZhHk.IsInsensitiveLike(carrierNameQuery, MatchMode.Anywhere)
            );
        }

        return query;
    }

    [HttpPost]
    [Route("/admin/seventeen/track/carrier/detail")]
    public IActionResult Detail(IdParam param) {
        var id = param.Id;

        SeventeenTrackCarrierInfo? seventeenTrackCarrier;

        if (id <= 0) {
            seventeenTrackCarrier = new SeventeenTrackCarrierInfo();
        } else {
            seventeenTrackCarrier = session.Get<SeventeenTrackCarrierInfo>(id);
            if (seventeenTrackCarrier == null) {
                throw new ValidationException($"指定的 17track运输商#{id} 不存在！");
            }
        }

        SeventeenTrackCarrierDetailModel model = new SeventeenTrackCarrierDetailModel();
        model.SeventeenTrackCarrierId = seventeenTrackCarrier.Id;

        model.CarrierKey = seventeenTrackCarrier.CarrierKey;
        model.CountryCode = seventeenTrackCarrier.CountryCode;
        model.CountryIso = seventeenTrackCarrier.CountryIso;
        model.Email = seventeenTrackCarrier.Email;
        model.Tel = seventeenTrackCarrier.Tel;
        model.Url = seventeenTrackCarrier.Url;
        model.CarrierName = seventeenTrackCarrier.CarrierName;
        model.NameZhCn = seventeenTrackCarrier.NameZhCn;
        model.NameZhHk = seventeenTrackCarrier.NameZhHk;
        model.CarrierGroup = seventeenTrackCarrier.CarrierGroup;


        return View("SeventeenTrackCarrierDetail", model);
    }

    [HttpPost]
    [Route("/admin/seventeen/track/carrier/sync")]
    [Transactional]
    public IActionResult Sync() {
        seventeenTrackCarrierService.SyncData();

        return Json(new JsonResultData {
            Success = true,
            Message = "运输上数据同步成功！",
        });
    }
}
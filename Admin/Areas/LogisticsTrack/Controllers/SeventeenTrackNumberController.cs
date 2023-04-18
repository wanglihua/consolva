using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.LogisticsTrack;
using shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackNumber;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using shunda.Domain.Pass.PassRes;
using shunda.Service.logisticsTrack.SeventeenTrack.Register;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsTrack.Controllers;

/// <summary>
/// 17TRACK注册单号 控制器
/// </summary>
[LogisticsTrackArea]
[AdminAuth(PassRes.LogisticsTrack.SeventeenTrackNumber)]
public class SeventeenTrackNumberController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly SeventeenTrackRegisterService seventeenTrackRegisterService;

    public SeventeenTrackNumberController(
        ISession session,
        SeventeenTrackRegisterService seventeenTrackRegisterService
    ) {
        this.session = session;
        this.seventeenTrackRegisterService = seventeenTrackRegisterService;
    }

    [HttpGet]
    [Route("/admin/logistics/track/seventeen/track/number")]
    public IActionResult Index() {
        SeventeenTrackNumberIndexModel model = new SeventeenTrackNumberIndexModel();

        model.OriginList = SeventeenTrackNumberOrigin.GetList();
        model.StatusList = SeventeenTrackNumberStatus.GetList();

        return View("SeventeenTrackNumberIndex", model);
    }

    [HttpGet]
    [Route("/admin/logistics/track/seventeen/track/number/register/{trackingNo}")]
    [Transactional]
    public IActionResult RegisterForTest(string trackingNo) {
        seventeenTrackRegisterService.Register(new List<SeventeenTrackRegisterRequestItem> {
            new SeventeenTrackRegisterRequestItem {
                Number = trackingNo,
            }
        });

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [HttpPost]
    [Route("/admin/logistics/track/seventeen/track/number/list")]
    public IActionResult List(SeventeenTrackNumberListQueryParam queryParam) {
        int totalCount = GetQueryOver(queryParam)
            // .Select(Projections.Count<SeventeenTrackNumberInfo>(e => seventeenTrackNumberAlias!.Id))
            .Select(Projections.RowCount())
            .SingleOrDefault<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        var listQuery = GetQueryOver(queryParam);
        if (listPager.IsAscending) {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Asc;
        } else {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Desc;
        }

        listQuery = listQuery.ThenByAlias(() => seventeenTrackNumberAlias!.Id).Desc;

        IList<SeventeenTrackNumberInfo> seventeenTrackNumberList = listQuery
            .Skip(listPager.Skip())
            .Take(listPager.Take())
            .List<SeventeenTrackNumberInfo>();

        /*
        foreach (SeventeenTrackNumberInfo seventeenTrackNumber in seventeenTrackNumberList) {
        }
        */

        SeventeenTrackNumberListModel model = new SeventeenTrackNumberListModel(listPager);
        model.SeventeenTrackNumberList = seventeenTrackNumberList;

        model.SeventeenTrackNumberAlias = nameof(seventeenTrackNumberAlias);

        return View("SeventeenTrackNumberList", model);
    }

    private SeventeenTrackNumberInfo? seventeenTrackNumberAlias = null;

    [NonAction]
    private IQueryOver<SeventeenTrackNumberInfo, SeventeenTrackNumberInfo> GetQueryOver(SeventeenTrackNumberListQueryParam queryParam) {
        var query = session.QueryOver<SeventeenTrackNumberInfo>(() => seventeenTrackNumberAlias!);

        if (string.IsNullOrWhiteSpace(queryParam.TrackingNoQuery) == false) {
            string trackingNoQuery = queryParam.TrackingNoQuery.Trim();
            query = query.Where(e => e.TrackingNo == trackingNoQuery);
        }

        if (queryParam.CarrierKeyQuery != null) {
            int carrierKeyQuery = queryParam.CarrierKeyQuery.Value;
            query = query.Where(e => e.CarrierKey == carrierKeyQuery);
        }

        if (string.IsNullOrWhiteSpace(queryParam.CarrierNameQuery) == false) {
            string carrierNameQuery = queryParam.CarrierNameQuery.Trim();
            query = query.Where(e => e.CarrierName.IsInsensitiveLike(carrierNameQuery, MatchMode.Anywhere));
        }

        if (queryParam.StatusQuery != null && queryParam.StatusQuery != SeventeenTrackNumberStatus.None) {
            var statusQuery = queryParam.StatusQuery;
            query = query.Where(e => e.Status == statusQuery);
        }

        return query;
    }

    [HttpPost]
    [Route("/admin/logistics/track/seventeen/track/number/detail")]
    public IActionResult Detail(IdParam param) {
        var id = param.Id;

        var seventeenTrackNumber = session.Get<SeventeenTrackNumberInfo>(id);
        if (seventeenTrackNumber == null) {
            throw new ValidationException($"指定的 17TRACK注册单号#{id} 不存在！");
        }

        SeventeenTrackNumberDetailModel model = new SeventeenTrackNumberDetailModel {
            SeventeenTrackNumberId = seventeenTrackNumber.Id,
            TrackingNo = seventeenTrackNumber.TrackingNo,
            CarrierKey = seventeenTrackNumber.CarrierKey,
            CarrierName = seventeenTrackNumber.CarrierName,
            Origin = seventeenTrackNumber.Origin,
            ErrorCode = seventeenTrackNumber.ErrorCode,
            ErrorMessage = seventeenTrackNumber.ErrorMessage,
            Status = seventeenTrackNumber.Status,
            OriginList = SeventeenTrackNumberOrigin.GetList(),
            StatusList = SeventeenTrackNumberStatus.GetList()
        };

        return View("SeventeenTrackNumberDetail", model);
    }
}
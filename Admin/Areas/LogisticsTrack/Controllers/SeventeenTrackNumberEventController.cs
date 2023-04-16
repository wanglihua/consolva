using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NLog;
using shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackNumberEvent;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.LogisticsTrack;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsTrack.Controllers;

/// <summary>
/// 17TRACK快递单事件 控制器
/// </summary>
[LogisticsTrackArea]
[AdminAuth(PassRes.LogisticsTrack.SeventeenTrackNumber)]
public class SeventeenTrackNumberEventController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public SeventeenTrackNumberEventController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/track/seventeen/track/number/event/{trackNumberId}")]
    public IActionResult Index(int trackNumberId) {
        SeventeenTrackNumberEventIndexModel model = new SeventeenTrackNumberEventIndexModel();
        model.TrackNumberId = trackNumberId;

        var trackNumber = session.Get<SeventeenTrackNumberInfo>(trackNumberId);
        if (trackNumber == null) {
            throw new ValidationException($"指定的17TRACK跟踪单号#{trackNumberId}不存在！");
        }

        model.TrackNumber = trackNumber;

        return View("SeventeenTrackNumberEventIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/track/seventeen/track/number/event/list")]
    public IActionResult List(SeventeenTrackNumberEventListQueryParam queryParam) {
        var listQuery = GetQueryOver(queryParam);
        listQuery = listQuery.OrderByAlias(() => seventeenTrackNumberEventAlias!.Id).Asc;

        IList<SeventeenTrackNumberEventInfo> seventeenTrackNumberEventList = listQuery
            .List<SeventeenTrackNumberEventInfo>();

        /*
        foreach (SeventeenTrackNumberEventInfo seventeenTrackNumberEvent in seventeenTrackNumberEventList) {
        }
        */

        SeventeenTrackNumberEventListModel model = new SeventeenTrackNumberEventListModel();
        model.SeventeenTrackNumberEventList = seventeenTrackNumberEventList;

        model.SeventeenTrackNumberEventAlias = nameof(seventeenTrackNumberEventAlias);
        model.TrackNumberAlias = nameof(trackNumberAlias);

        return View("SeventeenTrackNumberEventList", model);
    }

    private SeventeenTrackNumberEventInfo? seventeenTrackNumberEventAlias = null;
    private SeventeenTrackNumberInfo? trackNumberAlias = null;

    [NonAction]
    private IQueryOver<SeventeenTrackNumberEventInfo, SeventeenTrackNumberEventInfo> GetQueryOver(SeventeenTrackNumberEventListQueryParam queryParam) {
        var query = session.QueryOver<SeventeenTrackNumberEventInfo>(() => seventeenTrackNumberEventAlias!);
        // .JoinAlias(() => seventeenTrackNumberEventAlias!.TrackNumber, () => trackNumberAlias);

        int trackNumberId = queryParam.TrackNumberId;
        query = query.Where(e => e.TrackNumber.Id == trackNumberId);

        return query;
    }
}
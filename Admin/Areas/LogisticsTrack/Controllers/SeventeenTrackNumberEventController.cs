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

    [HttpPost]
    [Route("/admin/logistics/track/seventeen/track/number/event/by/track/number/id")]
    public IActionResult DialogByTrackNumberId(int trackNumberId) {
        SeventeenTrackNumberEventIndexModel model = new SeventeenTrackNumberEventIndexModel();

        var trackNumber = session.Get<SeventeenTrackNumberInfo>(trackNumberId);
        if (trackNumber == null) {
            throw new ValidationException($"指定的17TRACK跟踪单号#{trackNumberId}不存在！");
        }

        model.TrackNumber = trackNumber;

        SeventeenTrackNumberEventInfo? eventAlias = null;
        IList<SeventeenTrackNumberEventInfo> eventList = session
            .QueryOver<SeventeenTrackNumberEventInfo>(() => eventAlias!)
            .Where(e => eventAlias!.TrackNumber.Id == trackNumberId)
            .List<SeventeenTrackNumberEventInfo>();

        eventList = eventList.OrderBy(e => e.EventTime).ThenBy(e => e.Id).ToList();

        model.SeventeenTrackNumberEventList = eventList;

        return View("SeventeenTrackNumberEventIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/track/seventeen/track/number/event/by/tracking/no")]
    public IActionResult DialogByTrackingNo(string trackingNo) {
        if (string.IsNullOrWhiteSpace(trackingNo)) {
            throw new ValidationException("查看跟踪明细，国际单号不能为空！");
        }

        trackingNo = trackingNo.Trim();

        SeventeenTrackNumberEventIndexModel model = new SeventeenTrackNumberEventIndexModel();

        var trackNumber = session.Query<SeventeenTrackNumberInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();
        if (trackNumber == null) {
            throw new ValidationException($"指定的17TRACK跟踪国际单号#{trackingNo}不存在！");
        }

        int trackNumberId = trackNumber.Id;

        model.TrackNumber = trackNumber;

        SeventeenTrackNumberEventInfo? eventAlias = null;
        IList<SeventeenTrackNumberEventInfo> eventList = session
            .QueryOver<SeventeenTrackNumberEventInfo>(() => eventAlias!)
            .Where(e => eventAlias!.TrackNumber.Id == trackNumberId)
            .List<SeventeenTrackNumberEventInfo>();

        eventList = eventList.OrderBy(e => e.EventTime).ThenBy(e => e.Id).ToList();

        model.SeventeenTrackNumberEventList = eventList;

        return View("SeventeenTrackNumberEventIndex", model);
    }
}
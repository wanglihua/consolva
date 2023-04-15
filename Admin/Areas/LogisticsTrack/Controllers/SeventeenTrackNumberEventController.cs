using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.LogisticsTrack;
using shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackNumberEvent;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
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
    [Route("/admin/logistics/track/seventeen/track/number/event")]
    public IActionResult Index() {
        SeventeenTrackNumberEventIndexModel model = new SeventeenTrackNumberEventIndexModel();


        return View("SeventeenTrackNumberEventIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/track/seventeen/track/number/event/list")]
    public IActionResult List(SeventeenTrackNumberEventListQueryParam queryParam) {
        int totalCount = GetQueryOver(queryParam)
            // .Select(Projections.Count<SeventeenTrackNumberEventInfo>(e => seventeenTrackNumberEventAlias!.Id))
            .Select(Projections.RowCount())
            .SingleOrDefault<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        var listQuery = GetQueryOver(queryParam);
        if (listPager.IsAscending) {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Asc;
        } else {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Desc;
        }

        listQuery = listQuery.ThenByAlias(() => seventeenTrackNumberEventAlias!.Id).Desc;

        IList<SeventeenTrackNumberEventInfo> seventeenTrackNumberEventList = listQuery
            .Skip(listPager.Skip())
            .Take(listPager.Take())
            .List<SeventeenTrackNumberEventInfo>();

        /*
        foreach (SeventeenTrackNumberEventInfo seventeenTrackNumberEvent in seventeenTrackNumberEventList) {
        }
        */

        SeventeenTrackNumberEventListModel model = new SeventeenTrackNumberEventListModel(listPager);
        model.SeventeenTrackNumberEventList = seventeenTrackNumberEventList;

        model.SeventeenTrackNumberEventAlias = nameof(seventeenTrackNumberEventAlias);
        model.TrackNumberAlias = nameof(trackNumberAlias);

        return View("SeventeenTrackNumberEventList", model);
    }

    private SeventeenTrackNumberEventInfo? seventeenTrackNumberEventAlias = null;
    private SeventeenTrackNumberInfo? trackNumberAlias = null;

    [NonAction]
    private IQueryOver<SeventeenTrackNumberEventInfo, SeventeenTrackNumberEventInfo> GetQueryOver(SeventeenTrackNumberEventListQueryParam queryParam) {
        var query = session.QueryOver<SeventeenTrackNumberEventInfo>(() => seventeenTrackNumberEventAlias!)
            .JoinAlias(() => seventeenTrackNumberEventAlias!.TrackNumber, () => trackNumberAlias);

        return query;
    }
}
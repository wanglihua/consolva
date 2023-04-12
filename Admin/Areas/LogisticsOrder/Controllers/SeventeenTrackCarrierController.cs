using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Admin.Areas.LogisticsOrder.Models.SeventeenTrackCarrier;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

/// <summary>
/// 17track运输商 控制器
/// </summary>
[LogisticsOrderArea]
[AdminAuth(PassRes.LogisticsOrder.SeventeenTrackCarrier)]
public class SeventeenTrackCarrierController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public SeventeenTrackCarrierController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/order/seventeen/track/carrier")]
    public IActionResult Index() {
        SeventeenTrackCarrierIndexModel model = new SeventeenTrackCarrierIndexModel();

        return View("SeventeenTrackCarrierIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/seventeen/track/carrier/list")]
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

        return query;
    }

    [HttpPost]
    [Route("/admin/logistics/order/seventeen/track/carrier/detail")]
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
}
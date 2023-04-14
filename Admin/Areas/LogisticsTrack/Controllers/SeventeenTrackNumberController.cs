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

    public SeventeenTrackNumberController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/track/seventeen/track/number")]
    public IActionResult Index() {
        SeventeenTrackNumberIndexModel model = new SeventeenTrackNumberIndexModel();

        model.OriginList = SeventeenTrackNumberOrigin.GetList();
        model.StatusList = SeventeenTrackNumberStatus.GetList();

        return View("SeventeenTrackNumberIndex", model);
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

        return query;
    }

    [HttpPost]
    [Route("/admin/logistics/track/seventeen/track/number/detail")]
    public IActionResult Detail(IdParam param) {
        var id = param.Id;

        SeventeenTrackNumberInfo? seventeenTrackNumber;

        if (id <= 0) {
            seventeenTrackNumber = new SeventeenTrackNumberInfo();
        } else {
            seventeenTrackNumber = session.Get<SeventeenTrackNumberInfo>(id);
            if (seventeenTrackNumber == null) {
                throw new ValidationException($"指定的 17TRACK注册单号#{id} 不存在！");
            }
        }

        SeventeenTrackNumberDetailModel model = new SeventeenTrackNumberDetailModel();
        model.SeventeenTrackNumberId = seventeenTrackNumber.Id;

        model.TrackingNo = seventeenTrackNumber.TrackingNo;
        model.CarrierKey = seventeenTrackNumber.CarrierKey;
        model.CarrierName = seventeenTrackNumber.CarrierName;
        model.Origin = seventeenTrackNumber.Origin;
        model.ErrorCode = seventeenTrackNumber.ErrorCode;
        model.ErrorMessage = seventeenTrackNumber.ErrorMessage;
        model.Status = seventeenTrackNumber.Status;

        model.OriginList = SeventeenTrackNumberOrigin.GetList();
        model.StatusList = SeventeenTrackNumberStatus.GetList();

        return View("SeventeenTrackNumberDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/track/seventeen/track/number/save")]
    [Transactional]
    public IActionResult Save(SeventeenTrackNumberSaveModel model) {
        ValidationUtil.Validate<SeventeenTrackNumberSaveModelValidator, SeventeenTrackNumberSaveModel>(model);

        int seventeenTrackNumberId = model.SeventeenTrackNumberId;

        bool isCreate = seventeenTrackNumberId <= 0;

        SeventeenTrackNumberInfo seventeenTrackNumber;

        if (isCreate) {
            seventeenTrackNumber = new SeventeenTrackNumberInfo();
        } else {
            seventeenTrackNumber = session.Get<SeventeenTrackNumberInfo>(seventeenTrackNumberId);
            if (seventeenTrackNumber == null){
                throw new ValidationException($"指定的 17TRACK注册单号#{seventeenTrackNumberId} 不存在！");
            }
        }

        seventeenTrackNumber.TrackingNo = model.TrackingNo;
        seventeenTrackNumber.CarrierKey = model.CarrierKey;
        seventeenTrackNumber.CarrierName = model.CarrierName;
        seventeenTrackNumber.Origin = model.Origin;
        seventeenTrackNumber.ErrorCode = model.ErrorCode;
        seventeenTrackNumber.ErrorMessage = model.ErrorMessage;
        seventeenTrackNumber.Status = model.Status;

        if (isCreate) {
            session.Save(seventeenTrackNumber);
        } else {
            session.Update(seventeenTrackNumber);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
        });
    }

    [HttpPost]
    [Route("/admin/logistics/track/seventeen/track/number/delete")]
    [Transactional]
    public IActionResult Delete(IdListParam param) {
        param.Validate("请选择要删除的17TRACK注册单号！");

        var idList = param.IdList;

        foreach (int id in idList) {
            SeventeenTrackNumberInfo seventeenTrackNumber = session.Load<SeventeenTrackNumberInfo>(id);
            session.Delete(seventeenTrackNumber);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "删除成功！",
        });
    }
}
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderFirstMode;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using shunda.Admin.Services;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsTrack;
using shunda.Domain.Pass.PassRes;
using shunda.Service.LogisticsOrder;
using shunda.Service.logisticsTrack.SeventeenTrack.Register;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

/// <summary>
/// 物流订单头程 控制器
/// </summary>
[LogisticsOrderArea]
[AdminAuth(PassRes.LogisticsOrder.LogisticsOrderFirstMode)]
public class LogisticsOrderFirstModeController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly LogisticsOrderFirstModeExcelService logisticsOrderFirstModeExcelService;
    private readonly SeventeenTrackRegisterService seventeenTrackRegisterService;

    public LogisticsOrderFirstModeController(
        ISession session,
        LoginUserService loginUserService,
        LogisticsOrderFirstModeExcelService logisticsOrderFirstModeExcelService,
        SeventeenTrackRegisterService seventeenTrackRegisterService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.logisticsOrderFirstModeExcelService = logisticsOrderFirstModeExcelService;
        this.seventeenTrackRegisterService = seventeenTrackRegisterService;
    }

    [HttpGet]
    [Route("/admin/logistics/order/logistics/order/first/mode")]
    public IActionResult Index() {
        LogisticsOrderFirstModeIndexModel model = new LogisticsOrderFirstModeIndexModel();

        model.GoodsTypeList = LogisticsGoodsType.GetInnerList();
        model.FirstTypeList = LogisticsModeType.GetList();
        model.ChannelList = LogisticsModeChannel.GetList();

        return View("LogisticsOrderFirstModeIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/logistics/order/first/mode/list")]
    public IActionResult List(LogisticsOrderFirstModeListQueryParam queryParam) {
        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false) {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            var order = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCodeQuery)
                .Take(1)
                .SingleOrDefault();
            queryParam.OrderIdQuery = order?.Id ?? 0;
        }

        int totalCount = GetQueryOver(queryParam)
            // .Select(Projections.Count<LogisticsOrderFirstModeInfo>(e => logisticsOrderFirstModeAlias!.Id))
            .Select(Projections.RowCount())
            .SingleOrDefault<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        var listQuery = GetQueryOver(queryParam);
        if (listPager.IsAscending) {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Asc;
        } else {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Desc;
        }

        listQuery = listQuery.ThenByAlias(() => orderFirstModeAlias!.Id).Desc;

        IList<LogisticsOrderFirstModeInfo> logisticsOrderFirstModeList = listQuery
            .Skip(listPager.Skip())
            .Take(listPager.Take())
            .List<LogisticsOrderFirstModeInfo>();

        /*
        foreach (LogisticsOrderFirstModeInfo logisticsOrderFirstMode in logisticsOrderFirstModeList) {
        }
        */

        LogisticsOrderFirstModeListModel model = new LogisticsOrderFirstModeListModel(listPager);
        model.LogisticsOrderFirstModeList = logisticsOrderFirstModeList;

        model.OrderFirstModeAlias = nameof(orderFirstModeAlias);
        model.OrderAlias = nameof(orderAlias);
        model.FirstModeAlias = nameof(firstModeAlias);

        return View("LogisticsOrderFirstModeList", model);
    }

    private LogisticsOrderFirstModeInfo? orderFirstModeAlias = null;
    private LogisticsOrderInfo? orderAlias = null;
    private LogisticsModeInfo? firstModeAlias = null;

    [NonAction]
    private IQueryOver<LogisticsOrderFirstModeInfo, LogisticsOrderFirstModeInfo> GetQueryOver(LogisticsOrderFirstModeListQueryParam queryParam) {
        var query = session.QueryOver<LogisticsOrderFirstModeInfo>(() => orderFirstModeAlias!)
            // .JoinAlias(() => orderFirstModeAlias!.Order, () => orderAlias)
            .JoinAlias(() => orderFirstModeAlias!.FirstMode, () => firstModeAlias);

        if (queryParam.OrderIdQuery != null) {
            int orderIdQuery = queryParam.OrderIdQuery ?? 0;
            query = query.Where(e => e.Order.Id == orderIdQuery);
        }

        if (string.IsNullOrWhiteSpace(queryParam.FirstModeNameQuery) == false) {
            string firstModeNameQuery = queryParam.FirstModeNameQuery.Trim();
            query = query.Where(e => firstModeAlias!.Name.IsInsensitiveLike(firstModeNameQuery, MatchMode.Anywhere));
        }

        if (queryParam.GoodsTypeQuery != null && queryParam.GoodsTypeQuery != LogisticsGoodsType.None) {
            LogisticsGoodsType goodsTypeQuery = queryParam.GoodsTypeQuery;
            query = query.Where(e => e.GoodsType == goodsTypeQuery);
        }

        if (string.IsNullOrWhiteSpace(queryParam.TrackingNoQuery) == false) {
            string trackingNoQuery = queryParam.TrackingNoQuery.Trim();
            query = query.Where(e => e.TrackingNo == trackingNoQuery);
        }

        if (queryParam.BeginDateQuery != null) {
            DateTime beginDateQueryValue = queryParam.BeginDateQuery.Value;
            query = query.Where(e => e.CreateTime >= beginDateQueryValue);
        }

        if (queryParam.EndDateQuery != null) {
            DateTime endDateQueryValue = queryParam.EndDateQuery.Value.AddDays(1);
            query = query.Where(e => e.CreateTime < endDateQueryValue);
        }

        return query;
    }

    [HttpPost]
    [Route("/admin/logistics/order/logistics/order/first/mode/detail")]
    public IActionResult Detail(IdParam param) {
        var id = param.Id;

        var orderFirstMode = session.Get<LogisticsOrderFirstModeInfo>(id);
        if (orderFirstMode == null) {
            throw new ValidationException($"指定的 物流订单头程#{id} 不存在！");
        }

        LogisticsOrderFirstModeDetailModel model = new LogisticsOrderFirstModeDetailModel();
        model.OrderFirstModeId = orderFirstMode.Id;

        model.OrderCode = orderFirstMode.OrderCode;
        model.FirstModeName = orderFirstMode.FirstMode.Name;
        model.GoodsType = orderFirstMode.GoodsType;
        model.TrackingNo = orderFirstMode.TrackingNo;

        return View("LogisticsOrderFirstModeDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/logistics/order/first/mode/save")]
    [Transactional]
    public IActionResult Save(LogisticsOrderFirstModeSaveModel model) {
        ValidationUtil.Validate<LogisticsOrderFirstModeSaveModelValidator, LogisticsOrderFirstModeSaveModel>(model);

        int orderFirstModeId = model.OrderFirstModeId;

        LogisticsOrderFirstModeInfo orderFirstMode = session.Get<LogisticsOrderFirstModeInfo>(orderFirstModeId);
        if (orderFirstMode == null) {
            throw new ValidationException($"指定的 物流订单头程#{orderFirstModeId} 不存在！");
        }

        string trackingNo = model.TrackingNo;
        if (string.IsNullOrWhiteSpace(trackingNo)) {
            trackingNo = string.Empty;
        }

        trackingNo = trackingNo.Trim();

        if (orderFirstMode.TrackingNo != trackingNo) {
            orderFirstMode.TrackingNo = trackingNo;

            session.Update(orderFirstMode);
            session.Flush();

            if (string.IsNullOrWhiteSpace(trackingNo) == false) {
                var trackNumber = session.Query<SeventeenTrackNumberInfo>()
                    .Where(e => e.TrackingNo == trackingNo
                                && (e.Status == SeventeenTrackNumberStatus.Accepted || e.Status == SeventeenTrackNumberStatus.Completed))
                    .Take(1)
                    .SingleOrDefault();
                if (trackNumber == null) {
                    seventeenTrackRegisterService.Register(new List<SeventeenTrackRegisterRequestItem>() {
                        new SeventeenTrackRegisterRequestItem() {
                            Number = trackingNo,
                        }
                    });
                }
            }
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
        });
    }

    [HttpPost]
    [Route("/admin/logistics/order/logistics/order/first/mode/export")]
    public IActionResult Export(LogisticsOrderFirstModeListQueryParam queryParam) {
        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false) {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            var order = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCodeQuery)
                .Take(1)
                .SingleOrDefault();
            queryParam.OrderIdQuery = order?.Id ?? 0;
        }

        var listQuery = GetQueryOver(queryParam);
        listQuery = listQuery.OrderByAlias(() => orderFirstModeAlias!.Id).Asc;

        IList<LogisticsOrderFirstModeInfo> orderFirstModeList = listQuery
            .List<LogisticsOrderFirstModeInfo>();

        var excel = logisticsOrderFirstModeExcelService.Export(orderFirstModeList);

        string fileName = $"国际单号导出.xlsx";
        return File(excel, ContentType.Xlsx, fileName);
    }

    [HttpPost]
    [Route("/admin/logistics/order/logistics/order/first/mode/import")]
    public IActionResult Import(IFormFile excelFile) {
        int loginUserId = loginUserService.GetLoginUserId();
        logisticsOrderFirstModeExcelService.Import(excelFile, loginUserId);

        return Json(new JsonResultData {
            Success = true,
            Message = "导入成功！",
        });
    }
}
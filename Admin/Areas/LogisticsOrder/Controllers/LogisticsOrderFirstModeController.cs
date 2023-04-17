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
using shunda.Domain.Logistics;
using shunda.Domain.Pass.PassRes;
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

    public LogisticsOrderFirstModeController(ISession session) {
        this.session = session;
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

        orderFirstMode.TrackingNo = trackingNo;

        session.Update(orderFirstMode);
        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
        });
    }
}
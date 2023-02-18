using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using shunda.Client.Areas.Express.Models.ExpressCheckin;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressCheckinController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsOrderCreateSelectExpressService logisticsOrderCreateSelectExpressService;

    public ExpressCheckinController(ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsOrderCreateSelectExpressService logisticsOrderCreateSelectExpressService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsOrderCreateSelectExpressService = logisticsOrderCreateSelectExpressService;
    }

    [HttpPost]
    [Route("/client/express/checkin/list")]
    public IActionResult List([FromBody] ExpressCheckinListParam param) {
        ExpressCheckinListModel model = GetListData(param);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }

    [NonAction]
    private ExpressCheckinListModel GetListData(ExpressCheckinListParam param) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        // 这个不分页，一次性全部取出来
        var expressList = GetListQuery(param)
            .OrderByDescending(e => e.Id)
            .ToList();

        // int expressCount = GetListQuery(param).Count(); 这个不用再查询一次
        int expressCount = expressList.Count;

        // 已选择数据
        IList<int> selectedExpressIdList = session.Query<LogisticsOrderCreateSelectExpressInfo>()
            .Where(e => e.CustomerId == loginCustomerId)
            .Select(e => e.ExpressId)
            .ToList();

        IDictionary<int, bool> selectedExpressIdMap = selectedExpressIdList.ToDictionary(e => e, e => true);

        // 数量
        int expressWaitOrderTotalCount = 0;
        int expressCheckinTotalCount = 0;
        foreach (var express in expressList) {
            int expressId = express.Id;
            if (selectedExpressIdMap.ContainsKey(expressId)) {
                expressWaitOrderTotalCount++;
            } else {
                expressCheckinTotalCount++;
            }
        }

        // 物品数据
        IList<int> expressIdList = expressList.Select(e => e.Id).ToList();
        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();

        IDictionary<int, IList<ExpressGoodsInfo>> expressGoodsListMap = new Dictionary<int, IList<ExpressGoodsInfo>>();
        // 让每个 express id 都有记录
        foreach (int expressId in expressIdList) {
            expressGoodsListMap[expressId] = new List<ExpressGoodsInfo>();
        }

        foreach (ExpressGoodsInfo expressGoods in expressGoodsList) {
            int expressId = expressGoods.Express.Id;

            if (expressGoodsListMap.TryGetValue(expressId, out IList<ExpressGoodsInfo>? thisExpressGoodsList) == false) {
                thisExpressGoodsList = new List<ExpressGoodsInfo>();
            }

            thisExpressGoodsList.Add(expressGoods);

            expressGoodsListMap[expressId] = thisExpressGoodsList;
        }

        // 快递单备注，客户备注，仓库备注，客服备注
        var expressRemarkList = session.Query<ExpressRemarkInfo>()
            .Where(e => expressIdList.Contains(e.Express!.Id))
            .ToList();

        // 构建两个列表
        IList<ExpressCheckinListItem> expressWaitOrderList = new List<ExpressCheckinListItem>();
        IList<ExpressCheckinListItem> expressCheckinList = new List<ExpressCheckinListItem>();

        // 按原来顺序循环，按原来顺序添加到 item list 中
        foreach (ExpressInfo express in expressList) {
            int expressId = express.Id;

            string customerRemark = expressRemarkList
                .Where(e => e.Express != null
                            && e.Express.Id == expressId
                            && e.Type == ExpressRemarkType.Customer)
                .Take(1)
                .SingleOrDefault()?.Content ?? string.Empty;

            string warehouseRemark = expressRemarkList
                .Where(e => e.Express != null
                            && e.Express.Id == expressId
                            && e.Type == ExpressRemarkType.Warehouse)
                .Take(1)
                .SingleOrDefault()?.Content ?? string.Empty;

            string serviceRemark = expressRemarkList
                .Where(e => e.Express != null
                            && e.Express.Id == expressId
                            && e.Type == ExpressRemarkType.Service)
                .Take(1)
                .SingleOrDefault()?.Content ?? string.Empty;

            ExpressCheckinListItem item = new ExpressCheckinListItem {
                Id = express.Id,
                TrackingNo = express.TrackingNo,
                ExpressCompany = express.ExpressCompany,
                GoodsType = express.GoodsType.Name,
                GoodsValue = express.GoodsValue.ToString("#0.00"),
                Forbidden = express.GoodsType == LogisticsGoodsType.Forbidden,
                CheckinTrackingNo = express.CheckinTrackingNo,
                AddDate = express.AddTime.ToString("yyyy-MM-dd"),
                CustomerRemark = customerRemark,
                WarehouseRemark = warehouseRemark,
                ServiceRemark = serviceRemark,
            };

            // 一般这个判断肯定是成立的
            if (expressGoodsListMap.TryGetValue(expressId, out IList<ExpressGoodsInfo>? thisExpressGoodList)) {
                IList<ExpressCheckinGoodsListItem> goodsList = thisExpressGoodList
                    .Select(e => new ExpressCheckinGoodsListItem {
                        Id = e.Id,
                        GoodsName = e.Goods.Name,
                        Quantity = e.Quantity,
                        Unit = e.Unit,
                    })
                    .ToList();

                item.GoodsList = goodsList;
                item.GoodsCount = goodsList.Count;
            }

            if (selectedExpressIdMap.ContainsKey(expressId)) {
                item.WaitOrder = true;
                expressWaitOrderList.Add(item);
            } else {
                item.WaitOrder = false;
                expressCheckinList.Add(item);
            }
        }

        return new ExpressCheckinListModel {
            ExpressWaitOrderList = expressWaitOrderList,
            ExpressCheckinList = expressCheckinList,
            ExpressWaitOrderCount = expressWaitOrderList.Count,
            ExpressCheckinCount = expressCheckinList.Count,
            ExpressWaitOrderTotalCount = expressWaitOrderTotalCount,
            ExpressCheckinTotalCount = expressCheckinTotalCount,
            ExpressTotalCount = expressCount,
        };
    }

    [NonAction]
    private IQueryable<ExpressInfo> GetListQuery(ExpressCheckinListParam param) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var query = session.Query<ExpressInfo>()
            .Where(e => e.Customer!.Id == loginCustomerId
                        && e.Status == ExpressStatus.Checkin);

        if (string.IsNullOrWhiteSpace(param.Query) == false) {
            string queryStr = param.Query.Trim();
            query = query.Where(e => e.TrackingNo.Contains(queryStr) || e.CheckinTrackingNo.Contains(queryStr));
        }

        return query;
    }

    [HttpPost]
    [Route("/client/express/checkin/add/wait/order")]
    [Transactional]
    public IActionResult AddWaitOrder([FromBody] ExpressCheckinAddWaitOrderParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        int expressId = param.ExpressId;

        logisticsOrderCreateSelectExpressService.AddWaitOrder(expressId: expressId, customerId: loginCustomerId);

        ExpressCheckinListModel model = GetListData(param);

        return Json(new JsonResultData {
            Success = true,
            Message = "加入成功！",
            Data = model, // 同时获取列表数据，这样就不用客户端再获取一次列表数据
        });
    }

    [HttpPost]
    [Route("/client/express/checkin/add/wait/order/all")]
    [Transactional]
    public IActionResult AddWaitOrderAll([FromBody] ExpressCheckinAddWaitOrderAllParam param) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        logisticsOrderCreateSelectExpressService.AddWaitOrderAll(customerId: loginCustomerId);

        ExpressCheckinListModel model = GetListData(param);

        return Json(new JsonResultData {
            Success = true,
            Message = "全部加入成功！",
            Data = model, // 同时获取列表数据，这样就不用客户端再获取一次列表数据
        });
    }

    [HttpPost]
    [Route("/client/express/checkin/remove/wait/order")]
    [Transactional]
    public IActionResult RemoveWaitOrder([FromBody] ExpressCheckinRemoveWaitOrderParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        int expressId = param.ExpressId;

        logisticsOrderCreateSelectExpressService.RemoveWaitOrder(expressId: expressId, customerId: loginCustomerId);

        ExpressCheckinListModel model = GetListData(param);

        return Json(new JsonResultData {
            Success = true,
            Message = "移除成功！",
            Data = model, // 同时获取列表数据，这样就不用客户端再获取一次列表数据
        });
    }

    [HttpPost]
    [Route("/client/express/checkin/remove/wait/order/all")]
    [Transactional]
    public IActionResult RemoveWaitOrderAll([FromBody] ExpressCheckinRemoveWaitOrderAllParam param) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        logisticsOrderCreateSelectExpressService.RemoveWaitOrderAll(customerId: loginCustomerId);

        ExpressCheckinListModel model = GetListData(param);

        return Json(new JsonResultData {
            Success = true,
            Message = "全部移除成功！",
            Data = model, // 同时获取列表数据，这样就不用客户端再获取一次列表数据
        });
    }
}
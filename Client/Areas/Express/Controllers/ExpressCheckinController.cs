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

    public ExpressCheckinController(ISession session, LoginCustomerService loginCustomerService, LogisticsOrderCreateSelectExpressService logisticsOrderCreateSelectExpressService) {
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

        int expressCount = GetListQuery(param).Count();

        // 已选择数据
        IList<int> selectedExpressIdList = session.Query<LogisticsOrderCreateSelectExpressInfo>()
            .Where(e => e.CustomerId == loginCustomerId)
            .Select(e => e.ExpressId)
            .ToList();

        IDictionary<int, bool> selectedExpressIdMap = selectedExpressIdList.ToDictionary(e => e, e => true);

        // 物品数据
        IList<int> expressIdList = expressList.Select(e => e.Id).ToList();
        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();

        IDictionary<int, IList<ExpressGoodsInfo>> expressGoodsListMap = new Dictionary<int, IList<ExpressGoodsInfo>>();
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

        foreach (ExpressInfo express in expressList) // 按原来顺序循环，按原来顺序添加到 item list 中
        {
            int expressId = express.Id;

            string customerRemark = expressRemarkList
                .Where(e => e.Express != null
                            && e.Express.Id == expressId
                            && e.Type == ExpressRemarkType.Customer)
                .Take(1)
                .SingleOrDefault()?.Remark ?? string.Empty;

            string warehouseRemark = expressRemarkList
                .Where(e => e.Express != null
                            && e.Express.Id == expressId
                            && e.Type == ExpressRemarkType.Warehouse)
                .Take(1)
                .SingleOrDefault()?.Remark ?? string.Empty;

            string serviceRemark = expressRemarkList
                .Where(e => e.Express != null
                            && e.Express.Id == expressId
                            && e.Type == ExpressRemarkType.Service)
                .Take(1)
                .SingleOrDefault()?.Remark ?? string.Empty;

            ExpressCheckinListItem item = new ExpressCheckinListItem {
                Id = express.Id,
                TrackingNo = express.TrackingNo,
                ExpressCompany = express.ExpressCompany,
                GoodsType = express.GoodsType.Name,
                Forbidden = express.GoodsType == LogisticsGoodsType.Forbidden,
                CheckinTrackingNo = express.CheckinTrackingNo,
                AddDate = express.AddTime.ToString("yyyy-MM-dd"),
                CustomerRemark = customerRemark,
                WarehouseRemark = warehouseRemark,
                ServiceRemark = serviceRemark,
            };

            if (expressGoodsListMap.TryGetValue(expressId, out IList<ExpressGoodsInfo>? thisExpressGoodList)) // 一般这个判断肯定是成立的
            {
                IList<ExpressCheckinGoodsListItem> goodsList = thisExpressGoodList
                    .Select(e => new ExpressCheckinGoodsListItem {
                        Id = e.Id,
                        GoodsName = e.Goods.Name,
                        Quantity = e.Quantity,
                        Price = e.Price.ToString("#0.00"),
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
            ExpressTotalCount = expressCount,
        };
    }

    [NonAction]
    private IQueryable<ExpressInfo> GetListQuery(ExpressCheckinListParam param) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var query = session.Query<ExpressInfo>()
            .Where(e => e.Customer!.Id == loginCustomerId
                        && (e.Status == ExpressStatus.WaitOrder || e.Status == ExpressStatus.CheckIn));

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

    [HttpPost]
    [Route("/client/express/checkin/price")]
    public IActionResult Price([FromBody] IdParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int expressId = param.Id;
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        var goodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => e.Express.Id == expressId)
            .ToList();

        var model = new ExpressCheckinPriceModel {
            Id = express.Id,
            TrackingNo = express.TrackingNo,
            CheckinTrackingNo = express.CheckinTrackingNo,
            GoodsList = goodsList.OrderBy(e => e.Id)
                .Select(e => new ExpressCheckinGoodsListItem {
                    Id = e.Id,
                    GoodsName = e.Goods.Name,
                    Quantity = e.Quantity,
                    Unit = e.Unit,
                    Price = e.Price.ToString("#0.00"),
                })
                .ToList(),
        };

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }

    [HttpPost]
    [Route("/client/express/checkin/price/save")]
    [Transactional]
    public IActionResult PriceSave([FromBody] ExpressCheckinPriceSaveParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        var expressId = param.ExpressId;
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        if (param.GoodsPriceList == null || param.GoodsPriceList.Count == 0) {
            throw new ValidationException("物品价格列表不能为空！");
        }
        
        // 检查价格录入

        var goodsIdList = param.GoodsPriceList.Select(e => e.Id).ToList();
        var goodsList = session.Query<ExpressGoodsInfo>()
            .Where(e => goodsIdList.Contains(e.Id))
            .ToList();

        var goodsPriceList = param.GoodsPriceList;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}
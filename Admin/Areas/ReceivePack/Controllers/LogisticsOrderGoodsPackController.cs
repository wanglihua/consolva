using System.Text;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain;
using shunda.Domain.Express;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using shunda.Service.Express;
using shunda.Service.LogisticsOrder;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.ReceivePack.Controllers;

[ReceivePackArea]
[AdminAuth(PassRes.ReceivePack.OrderPack)]
public class LogisticsOrderGoodsPackController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly LogisticsOrderRemarkService logisticsOrderRemarkService;
    private readonly ExpressRemarkService expressRemarkService;
    private readonly LogisticsOrderPackBoxService logisticsOrderPackBoxService;
    private readonly LogisticsOrderPackBoxHistoryService logisticsOrderPackBoxHistoryService;
    private readonly ExpressLogService expressLogService;
    private readonly LogisticsOrderPackService logisticsOrderPackService;
    private readonly PassResService passResService;

    public LogisticsOrderGoodsPackController(ISession session,
        LoginUserService loginUserService,
        LogisticsOrderRemarkService logisticsOrderRemarkService,
        ExpressRemarkService expressRemarkService,
        LogisticsOrderPackBoxService logisticsOrderPackBoxService,
        LogisticsOrderPackBoxHistoryService logisticsOrderPackBoxHistoryService,
        ExpressLogService expressLogService,
        LogisticsOrderPackService logisticsOrderPackService,
        PassResService passResService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.logisticsOrderRemarkService = logisticsOrderRemarkService;
        this.expressRemarkService = expressRemarkService;
        this.logisticsOrderPackBoxService = logisticsOrderPackBoxService;
        this.logisticsOrderPackBoxHistoryService = logisticsOrderPackBoxHistoryService;
        this.expressLogService = expressLogService;
        this.logisticsOrderPackService = logisticsOrderPackService;
        this.passResService = passResService;
    }

    [HttpGet]
    [Route("/admin/logistics/order/goods/pack/{warehouseId:int}")]
    public IActionResult Index(int warehouseId) {
        WarehouseInfo warehouse = session.Get<WarehouseInfo>(warehouseId);
        if (warehouse == null) {
            throw new ValidationException("指定的仓库不存在！");
        }

        LogisticsOrderGoodsPackIndexModel model = new LogisticsOrderGoodsPackIndexModel();
        model.Warehouse = warehouse;

        return View("LogisticsOrderGoodsPackIndex", model);
    }

    /// <summary>
    /// 这个是选择订单的对话框
    /// </summary>
    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/order/list/dialog")]
    public IActionResult OrderListDialog(int warehouseId) {
        WarehouseInfo warehouse = session.Get<WarehouseInfo>(warehouseId);
        if (warehouse == null) {
            throw new ValidationException("指定的仓库不存在！");
        }

        IList<LogisticsOrderStatus> orderStatusList = new List<LogisticsOrderStatus>();
        orderStatusList.Add(LogisticsOrderStatus.WaitPack);
        orderStatusList.Add(LogisticsOrderStatus.Packed);

        LogisticsOrderGoodsPackOrderListDialogModel model = new LogisticsOrderGoodsPackOrderListDialogModel();
        model.CountryList = Country.GetNoChinaList();
        model.Warehouse = warehouse;
        model.OrderStatusList = orderStatusList;

        var firstModeTypeList = LogisticsModeType.GetList();
        model.FirstModeTypeList = firstModeTypeList;

        model.FirstModeList = session.Query<LogisticsModeInfo>()
            .Where(e => e.Cate == LogisticsModeCate.Customer
                        && firstModeTypeList.Contains(e.Type)
                        && e.InnerStatus != LogisticsModeStatus.Disabled)
            .OrderBy(e => e.Name)
            .ToList();

        model.GoodsTypeList = LogisticsGoodsType.GetCustomerList();

        return View("LogisticsOrderGoodsPackOrderListDialog", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/order/list/first/mode/select")]
    public IActionResult OrderListQueryFirstModeSelect(int countryId) {
        Country country = Country.GetById(countryId);

        LogisticsOrderGoodsPackOrderListDialogModel model = new LogisticsOrderGoodsPackOrderListDialogModel();

        var firstModeTypeList = LogisticsModeType.GetList();
        var query = session.Query<LogisticsModeInfo>()
            .Where(e => e.Cate == LogisticsModeCate.Customer
                        && firstModeTypeList.Contains(e.Type)
                        && e.InnerStatus != LogisticsModeStatus.Disabled);

        if (country != Country.None) {
            query = query.Where(e => e.Country == country);
        }

        model.FirstModeList = query
            .OrderBy(e => e.Name)
            .ToList();

        return View("LogisticsOrderGoodsPackOrderListQueryFirstModeSelect", model);
    }

    /// <summary>
    /// 这个是选择订单的对话框里面的列表数据
    /// </summary>
    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/order/list")]
    public IActionResult OrderList(LogisticsOrderGoodsPackOrderListQueryParam queryParam) {
        // TODO 这里的排序方式, 是不是可以参照 public IActionResult Order(LogisticsOrderGoodsPackOrderQueryParam queryParam) 里面的排序方式

        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsOrderInfo> orderList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(NHibernate.Criterion.Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsOrderInfo>();

        IList<int> orderIdList = orderList.Select(e => e.Id).ToList();

        var orderPackScheduleList = session.Query<LogisticsOrderPackScheduleInfo>()
            .Fetch(e => e.User)
            .Where(e => orderIdList.Contains(e.Order.Id))
            .ToList();

        foreach (var order in orderList) {
            int orderId = order.Id;
            var orderPackSchedule = orderPackScheduleList.Where(e => e.Order.Id == orderId).Take(1).SingleOrDefault();
            order.PackScheduleUserNameCache = orderPackSchedule?.User.UserName ?? string.Empty;
        }

        LogisticsOrderGoodsPackOrderListModel model = new LogisticsOrderGoodsPackOrderListModel(listPager);
        model.OrderList = orderList;

        return View("LogisticsOrderGoodsPackOrderList", model);
    }

    /// <summary>
    /// 这里的查询条件, 显示当前打包订单的时候使用, 显示对话框订单列表的时候也使用的
    /// </summary>
    /// <param name="queryParam"></param>
    /// <returns></returns>
    [NonAction]
    private ICriteria GetListCriteria(LogisticsOrderGoodsPackOrderListQueryParam queryParam) {
        LogisticsOrderPackDisplayType displayType = LogisticsOrderPackDisplayType.NotOthers;
        var orderPackConfig = session
            .Query<LogisticsOrderPackConfigInfo>()
            .Take(1)
            .SingleOrDefault();

        int packDelayMintes = 15; // 就是 15 分钟
        if (orderPackConfig != null) {
            if (orderPackConfig.DisplayType == LogisticsOrderPackDisplayType.None) {
                displayType = LogisticsOrderPackDisplayType.NotOthers;
            } else {
                displayType = orderPackConfig.DisplayType;
            }

            packDelayMintes = orderPackConfig.PackDelayMinutes;
            if (packDelayMintes < 0) {
                // 这个不判断，即使小于0，也不影响逻辑的
                packDelayMintes = 0;
            }
        }

        // string shelfAlias = "shelf";

        ICriteria criteria = session.CreateCriteria<LogisticsOrderInfo>();
        criteria = criteria.CreateAlias(nameof(LogisticsOrderInfo.PackSchedules), nameof(LogisticsOrderInfo.PackSchedules), JoinType.LeftOuterJoin);
        // criteria = criteria.CreateAlias(nameof(LogisticsOrderInfo.StorageLoc), nameof(LogisticsOrderInfo.StorageLoc)); // 有些订单起始仓位对应的仓位被删除了，这是表联接会造成列表中这个订单不显示
        criteria = criteria.CreateAlias(nameof(LogisticsOrderInfo.FirstMode), nameof(LogisticsOrderInfo.FirstMode));
        // criteria = criteria.CreateAlias($"{nameof(LogisticsOrderInfo.StorageLoc)}.{nameof(LogisticsOrderInfo.StorageLoc.Shelf)}", shelfAlias); // 有些订单起始仓位对应的仓位被删除了，这是表联接会造成列表中这个订单不显示
        criteria = criteria.SetReadOnly(true);

        // 禁止类的不能打包
        criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.GoodsType != LogisticsGoodsType.Forbidden));

        // 当前收货仓库的订单
        int warehouseId = queryParam.WarehouseId;
        criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.Warehouse.Id == warehouseId));

        // 状态
        criteria = criteria.Add(
            Restrictions.Or(
                Restrictions.Where<LogisticsOrderInfo>(e => e.OrderStatus == LogisticsOrderStatus.WaitPack),
                Restrictions.Where<LogisticsOrderInfo>(e => e.OrderStatus == LogisticsOrderStatus.Packed)
            ));

        if (queryParam.OrderStatusQuery == LogisticsOrderStatus.WaitPack) {
            // 取当前分配给当前用户的和未分配的
            int loginUserId = loginUserService.GetLoginUserId();
            bool isAdmin = loginUserService.IsAdmin;

            if (isAdmin == false) {
                if (displayType == LogisticsOrderPackDisplayType.NotOthers || displayType == LogisticsOrderPackDisplayType.None) {
                    criteria = criteria.Add(Restrictions.Or(
                        Restrictions.Eq($"{nameof(LogisticsOrderInfo.PackSchedules)}.{nameof(LogisticsOrderPackScheduleInfo.User)}.{nameof(LogisticsOrderPackScheduleInfo.User.Id)}", loginUserId), // 已排版的，排给自己的
                        Restrictions.IsNull($"{nameof(LogisticsOrderInfo.PackSchedules)}.{nameof(LogisticsOrderPackScheduleInfo.User)}.{nameof(LogisticsOrderPackScheduleInfo.User.Id)}") // 未排班的
                    ));
                }

                if (displayType == LogisticsOrderPackDisplayType.OnlyMine) {
                    criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsOrderInfo.PackSchedules)}.{nameof(LogisticsOrderPackScheduleInfo.User)}.{nameof(LogisticsOrderPackScheduleInfo.User.Id)}", loginUserId));
                }
            }

            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.OrderStatus == LogisticsOrderStatus.WaitPack));
        } else if (queryParam.OrderStatusQuery == LogisticsOrderStatus.Packed) {
            // 已打包的可以取分配给别人的
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.OrderStatus == LogisticsOrderStatus.Packed));
        }

        // 只有待打包的才需要考虑这个提交打包时间的问题
        if (queryParam.OrderStatusQuery == LogisticsOrderStatus.WaitPack) {
            // 空运设置 客户【提交打包】后，15分钟 可以进 打包池
            DateTime fifteenMinutesBefore = DateTime.Now.AddMinutes(-packDelayMintes);
            criteria = criteria.Add(Restrictions.Or(
                Restrictions.IsNotNull($"{nameof(LogisticsOrderInfo.PackSchedules)}.{nameof(LogisticsOrderPackScheduleInfo.User)}"), // 已排班的订单不受这个时间限制
                Restrictions.Where<LogisticsOrderInfo>(e => e.SubmitPackTime <= fifteenMinutesBefore)
            ));
        }

        // 先显示排班的订单
        // criteria = criteria.AddOrder(new Order($"{nameof(LogisticsOrderInfo.PackSchedules)}.{nameof(LogisticsOrderPackScheduleInfo.User)}", false));

        #region 页面其他手工查询条件

        // 订单编号查询
        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false) {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsOrderInfo>(e => e.Code).IsInsensitiveLike(orderCodeQuery, MatchMode.Anywhere));
        }

        if (queryParam.FirstModeIdQuery != 0) {
            int firstModeIdQuery = queryParam.FirstModeIdQuery;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.FirstMode.Id == firstModeIdQuery));
        }

        if (queryParam.GoodsTypeQuery != null && queryParam.GoodsTypeQuery != LogisticsGoodsType.None) {
            LogisticsGoodsType goodsTypeQuery = queryParam.GoodsTypeQuery;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.GoodsType == goodsTypeQuery));
        }

        if (queryParam.BeginDateQuery != null) {
            DateTime beginDateQueryValue = queryParam.BeginDateQuery.Value;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.SubmitPackTime >= beginDateQueryValue));
        }

        if (queryParam.EndDateQuery != null) {
            DateTime endDateQueryValue = queryParam.EndDateQuery.Value.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.SubmitPackTime < endDateQueryValue));
        }

        #endregion

        return criteria;
    }

    /// <summary>
    /// 获取显示当前打包的订单
    /// </summary>
    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/order")]
    public IActionResult Order(LogisticsOrderGoodsPackOrderQueryParam queryParam) {
        int? nextOrderId = queryParam.NextOrderId;
        int? preOrderId = queryParam.OrderId;
        int warehouseId = queryParam.WarehouseId;

        int loginUserId = loginUserService.GetLoginUserId();

        LogisticsOrderGoodsPackOrderModel model = new LogisticsOrderGoodsPackOrderModel();
        model.TrackingNoManualInputEnabled = true;

        // 就是这个URL对应界面打开后，当前打包的这个订单的ID
        int nextOrderIdValue = 0;
        if (nextOrderId.HasValue) {
            nextOrderIdValue = nextOrderId.Value;
        } else {
            LogisticsOrderPackSortType sortType = LogisticsOrderPackSortType.StorageLoc;
            LogisticsOrderPackDisplayType displayType = LogisticsOrderPackDisplayType.NotOthers;

            var orderPackConfig = session
                .Query<LogisticsOrderPackConfigInfo>()
                .Take(1)
                .SingleOrDefault();

            if (orderPackConfig != null) {
                sortType = orderPackConfig.SortType;

                if (orderPackConfig.DisplayType == LogisticsOrderPackDisplayType.None) {
                    displayType = LogisticsOrderPackDisplayType.NotOthers;
                } else {
                    displayType = orderPackConfig.DisplayType;
                }

                model.TrackingNoManualInputEnabled = orderPackConfig.TrackingNoManualInputEnabled;
            }

            string shelfAlias = "shelf";

            ICriteria criteria = session.CreateCriteria<LogisticsOrderInfo>();
            criteria = criteria.CreateAlias(nameof(LogisticsOrderInfo.PackSchedules), nameof(LogisticsOrderInfo.PackSchedules), JoinType.LeftOuterJoin);
            criteria = criteria.CreateAlias(nameof(LogisticsOrderInfo.StorageLoc), nameof(LogisticsOrderInfo.StorageLoc));
            criteria = criteria.CreateAlias($"{nameof(LogisticsOrderInfo.StorageLoc)}.{nameof(LogisticsOrderInfo.StorageLoc.Shelf)}", shelfAlias);
            criteria = criteria.SetReadOnly(true);

            // 禁止类的不能打包,
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.GoodsType != LogisticsGoodsType.Forbidden));

            // 当前收货仓库的订单
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.Warehouse.Id == warehouseId));

            // 状态
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.OrderStatus == LogisticsOrderStatus.WaitPack));

            // 取当前分配给当前用户的和未分配，或者只取分配给当前用户的
            bool isAdmin = loginUserService.IsAdmin;

            if (isAdmin == false) // admin 显示所有
            {
                if (displayType == LogisticsOrderPackDisplayType.NotOthers || displayType == LogisticsOrderPackDisplayType.None) {
                    criteria = criteria.Add(Restrictions.Or(
                        Restrictions.Eq($"{nameof(LogisticsOrderInfo.PackSchedules)}.{nameof(LogisticsOrderPackScheduleInfo.User)}.{nameof(LogisticsOrderPackScheduleInfo.User.Id)}", loginUserId),
                        Restrictions.IsNull($"{nameof(LogisticsOrderInfo.PackSchedules)}.{nameof(LogisticsOrderPackScheduleInfo.User)}.{nameof(LogisticsOrderPackScheduleInfo.User.Id)}")
                    ));
                }

                if (displayType == LogisticsOrderPackDisplayType.OnlyMine) {
                    criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsOrderInfo.PackSchedules)}.{nameof(LogisticsOrderPackScheduleInfo.User)}.{nameof(LogisticsOrderPackScheduleInfo.User.Id)}", loginUserId));
                }
            }

            // 空运设置 客户【提交打包】后，15分钟 可以进 打包池
            DateTime fifteenMinutesBefore = DateTime.Now.AddMinutes(-15);
            criteria = criteria.Add(Restrictions.Or(
                Restrictions.IsNotNull($"{nameof(LogisticsOrderInfo.PackSchedules)}.{nameof(LogisticsOrderPackScheduleInfo.User)}"), // 已排班的订单不受这个时间限制
                Restrictions.Where<LogisticsOrderInfo>(e => e.SubmitPackTime <= fifteenMinutesBefore)
            ));

            // 先显示排班的订单
            criteria = criteria.AddOrder(new Order($"{nameof(LogisticsOrderInfo.PackSchedules)}.{nameof(LogisticsOrderPackScheduleInfo.User)}", false));

            if (sortType == LogisticsOrderPackSortType.CreateTime) {
                criteria = criteria.AddOrder(new Order(nameof(LogisticsOrderInfo.CreateTime), true));
                criteria = criteria.AddOrder(new Order($"{shelfAlias}.{nameof(LogisticsOrderInfo.StorageLoc.Shelf.OrderNum)}", true));
                criteria = criteria.AddOrder(new Order($"{nameof(LogisticsOrderInfo.StorageLoc)}.{nameof(LogisticsOrderInfo.StorageLoc.RowNum)}", true));
                criteria = criteria.AddOrder(new Order($"{nameof(LogisticsOrderInfo.StorageLoc)}.{nameof(LogisticsOrderInfo.StorageLoc.ColNum)}", true));
            } else if (sortType == LogisticsOrderPackSortType.SubmitPackTime) {
                criteria = criteria.AddOrder(new Order(nameof(LogisticsOrderInfo.SubmitPackTime), true));
                criteria = criteria.AddOrder(new Order($"{shelfAlias}.{nameof(LogisticsOrderInfo.StorageLoc.Shelf.OrderNum)}", true));
                criteria = criteria.AddOrder(new Order($"{nameof(LogisticsOrderInfo.StorageLoc)}.{nameof(LogisticsOrderInfo.StorageLoc.RowNum)}", true));
                criteria = criteria.AddOrder(new Order($"{nameof(LogisticsOrderInfo.StorageLoc)}.{nameof(LogisticsOrderInfo.StorageLoc.ColNum)}", true));
            } else if (sortType == LogisticsOrderPackSortType.StorageLoc) {
                criteria = criteria.AddOrder(new Order($"{shelfAlias}.{nameof(LogisticsOrderInfo.StorageLoc.Shelf.OrderNum)}", true));
                criteria = criteria.AddOrder(new Order($"{nameof(LogisticsOrderInfo.StorageLoc)}.{nameof(LogisticsOrderInfo.StorageLoc.RowNum)}", true));
                criteria = criteria.AddOrder(new Order($"{nameof(LogisticsOrderInfo.StorageLoc)}.{nameof(LogisticsOrderInfo.StorageLoc.ColNum)}", true));
            }

            criteria = criteria.SetProjection(
                Projections.Distinct(
                    Projections.Property<LogisticsOrderInfo>(e => e.Id)
                )
            );

            IList<int> orderIdList = criteria.List<int>();

            if (preOrderId == null || preOrderId.Value == 0) {
                nextOrderIdValue = orderIdList.Take(1).SingleOrDefault();
            } else {
                bool foundOrderId = false;
                foreach (int oId in orderIdList) {
                    if (foundOrderId) {
                        // 就是找到后的下一个
                        nextOrderIdValue = oId;
                        break;
                    }

                    if (oId == preOrderId.Value) {
                        foundOrderId = true;
                    }
                }

                // 如果 orderIdList 里面不包含 orderId；或者包含的，但是列表的最后一个
                // 如果一个订单打包完了，点击下一个的时候，应该就是不包含了的情况
                if (nextOrderIdValue == 0) {
                    nextOrderIdValue = orderIdList.Take(1).FirstOrDefault();
                }
            }
        }

        var logisticsOrder = session.Get<LogisticsOrderInfo>(nextOrderIdValue);
        if (logisticsOrder == null) {
            logisticsOrder = new LogisticsOrderInfo(); // 对应 model 中成员的不为空！
        }

        model.IsAdmin = loginUserService.IsAdmin;

        model.Order = logisticsOrder;

        if (logisticsOrder.Id > 0) {
            IList<ExpressInfo> expressList = session.Query<ExpressInfo>()
                .Fetch(e => e.StorageLoc)
                .ThenFetch(e => e.Shelf)
                .Where(e => e.Order!.Id == nextOrderIdValue)
                .ToList();
            if (expressList.Count <= 0) {
                throw new ValidationException($"订单{logisticsOrder.Code}快递单列表为空，数据有问题！");
            }

            IList<ExpressGoodsInfo> orderGoodsList = session.Query<ExpressGoodsInfo>()
                .Fetch(e => e.Express)
                .Fetch(e => e.Goods)
                .Where(e => e.Express.Order!.Id == nextOrderIdValue)
                .ToList();
            foreach (ExpressInfo orderGoodsExpress in expressList) {
                IList<ExpressGoodsInfo> expressGoodsList = orderGoodsList.Where(e => e.Express.Id == orderGoodsExpress.Id).ToList();
                IList<string> goodsNameList = expressGoodsList.Select(e => e.Goods.Name).Distinct().ToList();
                orderGoodsExpress.GoodsNamesCache = string.Join(", ", goodsNameList);
            }

            model.GoodsTypeList = LogisticsGoodsType.GetInnerList(); // 这是使用 GetCustomerList() 也可以，但用 Inner 也无妨，万一订单中有不符合要求的物品

            model.GoodsTypePackCompleteDict = new Dictionary<LogisticsGoodsType, bool>();
            foreach (LogisticsGoodsType goodsType in model.GoodsTypeList) {
                int goodsTypeNotPackedCount = expressList.Where(e => e.GoodsType == goodsType && e.Status != ExpressStatus.Pack).Count();
                if (goodsTypeNotPackedCount == 0) {
                    model.GoodsTypePackCompleteDict[goodsType] = true;
                } else {
                    model.GoodsTypePackCompleteDict[goodsType] = false;
                }
            }

            // 按仓位号排序，使用的是内存中排序的方式
            expressList = expressList
                .OrderBy(e => e.StorageLoc.Shelf.OrderNum)
                .ThenBy(e => e.StorageLoc.RowNum)
                .ThenBy(e => e.StorageLoc.ColNum)
                .ToList();

            model.ExpressList = expressList;
            model.OrderTotalExpressPackageCount = expressList.Sum(e => e.PackageCount);

            // Common count
            var expressCommonTypeList = expressList
                .Where(e => e.GoodsType == LogisticsGoodsType.Common)
                .ToList();
            int commonPackageCount = 0;
            if (expressCommonTypeList.Count != 0) {
                commonPackageCount = expressCommonTypeList.Sum(e => e.PackageCount);
            }

            model.GoodsTypeCommonExpressPackageCount = commonPackageCount;

            // 订单备注 快递单备注。查询列表，同一个请求中还会做同样的查询的，所以二级缓存下没有性能问题
            var orderRemarkList = logisticsOrderRemarkService.GetOrderRemarkList(nextOrderIdValue);
            var expressRemarkList = expressRemarkService.GetOrderExpressRemarkList(nextOrderIdValue);
            model.RemarkTotalCount = orderRemarkList.Count + expressRemarkList.Count;

            model.IsOrderPackComplete = logisticsOrder.OrderStatus == LogisticsOrderStatus.Packed;

            {
                var orderPackSchedule = session.Query<LogisticsOrderPackScheduleInfo>()
                    .Fetch(e => e.User)
                    .Where(e => e.Order.Id == nextOrderIdValue)
                    .Take(1)
                    .SingleOrDefault();

                if (orderPackSchedule != null) {
                    model.PackScheduleUserName = $"{orderPackSchedule.User?.LoginName}({orderPackSchedule.User?.UserName})";
                } else {
                    model.PackScheduleUserName = "未排班";
                }
            }
        }

        return View("LogisticsOrderGoodsPackOrder", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/search/express")]
    public IActionResult SearchExpress(int orderId, int goodsTypeId, string expressTrackingNo) {
        if (string.IsNullOrWhiteSpace(expressTrackingNo)) {
            throw new ValidationException("快递单号不能为空！");
        }

        expressTrackingNo = expressTrackingNo.ToUpper();
        LogisticsGoodsType goodsType = LogisticsGoodsType.GetById(goodsTypeId);

        /*
            已成功查找到快递号并选择！本快递单号入库包裹数量为 1，请确保下架包裹数量正确！
            未能找到录入的快递单号，请核对
         */

        var orderGoodsExpress = session
            .Query<ExpressInfo>()
            .Fetch(e => e.Order)
            .Where(e => e.Order!.Id == orderId && e.TrackingNo == expressTrackingNo)
            .OrderBy(e => e.Status)
            .Take(1)
            .SingleOrDefault();

        // 如果订单快递信息中没找到快递单号，就尝试用入库跟踪号来匹配快递单号
        if (orderGoodsExpress == null) {
            string checkinTrackingNo = expressTrackingNo;
            orderGoodsExpress = session
                .Query<ExpressInfo>()
                .Fetch(e => e.Order)
                .Where(e => e.Order!.Id == orderId && e.CheckinTrackingNo == checkinTrackingNo)
                .OrderBy(e => e.Status)
                .Take(1)
                .SingleOrDefault();
        }

        if (orderGoodsExpress == null) {
            return Json(new Dictionary<string, object> {
                { "success", false },
                { "expressInOrder", false },
                { "message", "未能在当前订单中找到录入的快递单号，请核对！" }
            });
        }

        string message = $"{expressTrackingNo} 已成功查找到快递号并选择！本快递单号入库包裹数量为 {orderGoodsExpress.PackageCount}，请确保下架包裹数量正确！";

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "packageCount", orderGoodsExpress.PackageCount },
            { "expressId", orderGoodsExpress.Id },
            { "expressTrackingNo", orderGoodsExpress.TrackingNo },
            { "message", message }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/order/pack/list")]
    public IActionResult OrderPackList(int orderId, int goodsTypeId, bool repack) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        LogisticsGoodsType goodsType = LogisticsGoodsType.GetById(goodsTypeId);

        var orderPackQuery = session
            .Query<LogisticsOrderPackInfo>()
            .Where(e => e.Order.Id == orderId);

        IList<LogisticsOrderPackInfo> orderPackList = orderPackQuery.ToList();
        foreach (var orderPack in orderPackList) {
            // 用这这个方式，不用交叉表，效率反倒会更高
            NHibernateUtil.Initialize(orderPack.OrderFirstMode);
            NHibernateUtil.Initialize(orderPack.OrderFirstMode.FirstMode);
        }

        LogisticsOrderGoodsPackOrderPackListModel model = new LogisticsOrderGoodsPackOrderPackListModel();

        // 判断该类型是不是都是打包状态
        var goodsTypeNotPackedCountQuery = session.Query<ExpressInfo>()
            .Where(e => e.Order!.Id == orderId && e.Status != ExpressStatus.Pack);

        int goodsTypeNotPackedCount = goodsTypeNotPackedCountQuery.Count();

        if (goodsTypeNotPackedCount == 0) {
            model.GoodsTypePackComplete = true;
        } else {
            model.GoodsTypePackComplete = false;
        }

        model.GoodsType = goodsType;
        model.OrderPackList = orderPackList;
        model.Repack = repack;

        model.GoodsTypeList = LogisticsGoodsType.GetCustomerList();

        Country country = order.Country;
        model.FirstModeList = session.Query<LogisticsModeInfo>()
            .Where(e => e.Country == country
                        && e.Cate == LogisticsModeCate.Customer
                        && e.Status == LogisticsModeStatus.Enabled)
            .ToList();

        // 物品类型，头程物流 下拉默认选择
        var orderFirstModeRecent = session.Query<LogisticsOrderFirstModeInfo>()
            .Where(e => e.Order.Id == orderId)
            .OrderByDescending(e => e.Id)
            .Take(1)
            .SingleOrDefault();
        if (orderFirstModeRecent != null) {
            model.SelectGoodsType = orderFirstModeRecent.GoodsType;
            model.SelectFirstModeId = orderFirstModeRecent.FirstMode.Id;
        } else {
            model.SelectGoodsType = order.GoodsType;
            model.SelectFirstModeId = order.FirstMode.Id;
        }

        return View("LogisticsOrderGoodsPackOrderPackList", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/save/pack")]
    [Transactional]
    public IActionResult SavePack(LogisticsOrderGoodsPackSavePackParam param) {
        ValidationUtil.Validate<LogisticsOrderGoodsPackSavePackParamValidator, LogisticsOrderGoodsPackSavePackParam>(param);

        // if (!ModelState.IsValid) {
        //     throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        // }
        //
        // if (param.Width <= 0 || param.Length <= 0 || param.Height <= 0 || param.Weight <= 0) {
        //     throw new ValidationException("长宽高重量数据必须大于0！");
        // }
        //
        // if (param.PackageCount <= 0) {
        //     throw new ValidationException("箱数必须大于0！");
        // }
        //
        // if (param.GoodsType == LogisticsGoodsType.None) {
        //     throw new ValidationException("物品类型不能为空！");
        // }
        //
        // if (param.FirstModeId <= 0) {
        //     throw new ValidationException("头程物流方式不能为空！");
        // }

        int orderId = param.OrderId;

        int loginUserId = loginUserService.GetLoginUserId();

        UserInfo loginUser = session.Get<UserInfo>(loginUserId);

        IList<string> trackingNoList = param.TrackingNoList;
        // if (trackingNoList == null || trackingNoList.Count == 0) {
        //     throw new ValidationException("请扫描或输入快递单号，再保存打包信息！");
        // }

        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException("指定的订单不存在！");
        }

        int firstModeId = param.FirstModeId;
        var firstMode = session.Get<LogisticsModeInfo>(param.FirstModeId);
        if (firstMode == null) {
            throw new ValidationException($"指定的物流方式#{param.FirstModeId}不存在！");
        }

        // 一次性获取订单的所有快递单备用
        var expressList = session.Query<ExpressInfo>()
            .Where(e => e.Order!.Id == orderId)
            .ToList();

        // 获取物品类型快递单号列表
        IList<ExpressInfo> goodsTypeGoodsExpressList = expressList; // 默认取所有，一般空运时就是所有

        IList<string> goodsTypeExpressTrackingNoList = goodsTypeGoodsExpressList
            .Select(e => e.TrackingNo)
            .ToList();

        int expressCountNotInGoodsType = param.TrackingNoList.Where(e => goodsTypeExpressTrackingNoList.Contains(e) == false).Count();
        if (expressCountNotInGoodsType != 0) {
            throw new ValidationException("快递单号不全属于当前订单当前物品类型！");
        }

        // 检查是不是有已经打包状态的
        IList<ExpressInfo> orderExpressToPackList = expressList
            .Where(e => trackingNoList.Contains(e.TrackingNo))
            .ToList();

        IList<string> packedExpressTrackingNoList = orderExpressToPackList
            .Where(e => e.Status == ExpressStatus.Pack)
            .Select(e => e.TrackingNo)
            .ToList();

        if (packedExpressTrackingNoList.Count != 0) {
            throw new ValidationException($"快递单号{string.Join(",", packedExpressTrackingNoList)}，已经是打包状态，不能继续打包！");
        }

        LogisticsOrderPackInfo orderPack = new LogisticsOrderPackInfo();

        bool isCreate = true;
        if (param.PackId > 0) {
            isCreate = false;
            orderPack = session.Get<LogisticsOrderPackInfo>(param.PackId);
            if (orderPack == null) {
                throw new ValidationException("指定的打包数据不存在！");
            }
        }

        // 判断物品类型和物流方式的组合
        var firstModeGoodsTypeList = session.Query<LogisticsModeGoodsTypeInfo>()
            .Where(e => e.LogisticsMode.Id == firstModeId)
            .Select(e => e.GoodsType)
            .ToList();

        if (firstModeGoodsTypeList.Contains(param.GoodsType) == false) {
            throw new ValidationException($"物流方式{firstMode.Name}不支持物品类型{param.GoodsType.Name}！");
        }

        var orderFirstMode = session.Query<LogisticsOrderFirstModeInfo>()
            .Where(e => e.Order.Id == orderId
                        && e.FirstMode.Id == firstModeId)
            .Take(1)
            .SingleOrDefault();

        if (orderFirstMode == null) {
            orderFirstMode = new LogisticsOrderFirstModeInfo {
                Order = order,
                OrderCode = order.Code,
                GoodsType = param.GoodsType,
                FirstMode = firstMode,
                FirstType = firstMode.Type,
                Channel = firstMode.Channel,
            };

            session.Save(orderFirstMode);
            session.Flush();
        }

        orderPack.Order = order;
        orderPack.OrderFirstMode = orderFirstMode;
        orderPack.GoodsType = param.GoodsType;
        orderPack.Length = param.Length;
        orderPack.Width = param.Width;
        orderPack.Height = param.Height;
        orderPack.Weight = param.Weight;
        orderPack.PackageCount = param.PackageCount;
        orderPack.PackUser = loginUser;

        DateTime now = DateTime.Now;
        orderPack.PackTime = now;

        if (isCreate) {
            session.Save(orderPack);
        } else {
            session.Update(orderPack);
        }

        session.Flush();

        // 保存打包快递单号

        foreach (ExpressInfo express in orderExpressToPackList) {
            var orderPackExpress = session
                .Query<LogisticsOrderPackExpressInfo>()
                .Where(e => e.Order.Id == orderId && e.OrderPack.Id == orderPack.Id && e.Express.Id == express.Id)
                .Take(1)
                .SingleOrDefault();

            if (orderPackExpress == null) {
                orderPackExpress = new LogisticsOrderPackExpressInfo();

                orderPackExpress.Order = session.Load<LogisticsOrderInfo>(orderId);
                orderPackExpress.OrderPack = orderPack;
                orderPackExpress.Express = express;

                session.Save(orderPackExpress);
            }

            express.ShelfStatus = ExpressShelfStatus.OffShelf;
            express.DumpStatus = ExpressDumpStatus.NotDump;
            express.Status = ExpressStatus.Pack;
            express.PackTime = now;
            express.PackUser = loginUser;

            session.Update(express);
        }

        session.Flush();

        // 保存箱号

        IList<LogisticsOrderPackBoxInfo> orderPackBoxList = new List<LogisticsOrderPackBoxInfo>();

        int packageCount = orderPack.PackageCount;
        for (int index = 0; index < packageCount; index++) {
            LogisticsOrderPackBoxInfo orderPackBox = new LogisticsOrderPackBoxInfo();
            orderPackBox.Order = order;
            orderPackBox.OrderPack = orderPack;
            orderPackBox.BoxNum = logisticsOrderPackBoxService.GetNextBoxNum(orderId);
            orderPackBox.Status = LogisticsOrderPackBoxStatus.Pack;

            session.Save(orderPackBox);
            session.Flush();

            orderPackBoxList.Add(orderPackBox);
        }

        // 箱号历史
        foreach (LogisticsOrderPackBoxInfo packBox in orderPackBoxList) {
            logisticsOrderPackBoxHistoryService.AddHistory(packBox, LogisticsOrderPackBoxHistoryType.Pack, LogisticsOrderPackBoxStatus.Pack, LogisticsOrderPackBoxStatus.Pack, loginUserId);
        }

        session.Flush();

        // 打包警告信息

        StringBuilder warningMessageBuilder = new StringBuilder();

        decimal orderPackLength = orderPack.Length;
        decimal orderPackWidth = orderPack.Width;
        decimal orderPackHeight = orderPack.Height;
        decimal orderPackWeight = orderPack.Weight;

        // 检查订单重复雷同数据
        int orderPackId = orderPack.Id;
        LogisticsGoodsType orderPackGoodsType = orderPack.GoodsType;
        int orderPackPackageCount = orderPack.PackageCount;

        int duplicatePackDataCount = session.Query<LogisticsOrderPackInfo>()
            .Where(e => e.Order.Id == orderId
                        && e.Id != orderPackId
                        && e.GoodsType == orderPackGoodsType
                        && e.Length == orderPackLength
                        && e.Width == orderPackWidth
                        && e.Height == orderPackHeight
                        && e.Weight == orderPackWeight
                        && e.PackageCount == orderPackPackageCount)
            .Count();

        bool hasDuplicatePackData = duplicatePackDataCount > 0;
        if (hasDuplicatePackData) {
            warningMessageBuilder.Append("当前打包数据保存成功, 但订单中存在与当前打包数据雷同的数据，请核查！<br /> 如有必要，请移除重复打包数据，并补打正确箱号标签！");
        }

        // 检查是否有录入数据异常
        bool hasLengthUnusual = false;
        if (orderPackWidth > 10_00m || orderPackLength > 10_00m || orderPackHeight > 10_00m) // 就是长度大于 10米
        {
            hasLengthUnusual = true;

            if (warningMessageBuilder.Length > 0) {
                warningMessageBuilder.Append("<br /><br />");
            }

            warningMessageBuilder.Append("长宽高数据有异常，请核查！");
        }

        bool hasWeightUnusual = false;
        decimal volumeWeight = (orderPackWidth * orderPackLength * orderPackHeight) / 5000; // 就是一个立方算 200公斤
        if (orderPackWeight > volumeWeight * 3) // 如果重量是体积重的 3 倍就警告
        {
            hasWeightUnusual = true;

            if (warningMessageBuilder.Length > 0) {
                warningMessageBuilder.Append("<br /><br />");
            }

            warningMessageBuilder.Append("重量数据有异常，请核查！");
        }

        // 快递单号日志
        foreach (ExpressInfo express in orderExpressToPackList) {
            expressLogService.UserLog(express.Id, ExpressLogType.Pack, loginUserId);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "当前打包数据保存成功！" },
            { "orderPackId", orderPack.Id },
            { "hasPackWarning", hasDuplicatePackData || hasLengthUnusual || hasWeightUnusual },
            { "packWarningMessage", warningMessageBuilder.ToString() },
        });
    }

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/remove/pack")]
    [Transactional]
    public IActionResult RemovePack(LogisticsOrderGoodsPackRemovePackParam param) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        IList<string> trackingNoList = param.TrackingNoList;
        if (trackingNoList == null || trackingNoList.Count == 0) {
            throw new ValidationException("请扫描或输入快递单号，再移除打包信息！");
        }

        int orderId = param.OrderId;
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException("指定的订单不存在！");
        }

        LogisticsGoodsType goodsType = param.GoodsType;

        int orderPackId = param.PackId;

        // 获取订单快递单号列表
        IList<ExpressInfo> goodsTypeExpressList = session
            .Query<ExpressInfo>()
            .Where(e => e.Order!.Id == orderId && e.GoodsType == goodsType)
            .ToList();

        IList<string> goodsTypeTrackingNoList = goodsTypeExpressList
            .Select(e => e.TrackingNo)
            .ToList();

        int expressCountNotInGoodsType = param.TrackingNoList.Where(e => goodsTypeTrackingNoList.Contains(e) == false).Count();
        if (expressCountNotInGoodsType != 0) {
            throw new ValidationException("快递单号不全属于当前物品类型！");
        }

        LogisticsOrderPackInfo orderPack = session.Get<LogisticsOrderPackInfo>(orderPackId);
        if (orderPack == null) {
            throw new ValidationException("指定的打包数据不存在！");
        }

        if (orderPack.Order.Id != orderId || orderPack.GoodsType != goodsType) {
            throw new ValidationException("指定的打包数据跟指定的订单或物品类型不匹配！");
        }

        // 先删除订单打包管理关联快递单号

        IList<LogisticsOrderPackExpressInfo> orderPackExpressList = session
            .Query<LogisticsOrderPackExpressInfo>()
            .Where(e => e.Order.Id == orderId && e.OrderPack.Id == orderPackId)
            .ToList();

        foreach (LogisticsOrderPackExpressInfo orderPackExpress in orderPackExpressList) {
            session.Delete(orderPackExpress);
        }

        session.Flush();

        IList<LogisticsOrderPackBoxInfo> orderPackBoxList = session.Query<LogisticsOrderPackBoxInfo>()
            .Where(e => e.Order.Id == orderId && e.OrderPack.Id == orderPackId)
            .ToList();

        // 删除箱号历史
        IList<int> packBoxIdList = orderPackBoxList.Select(e => e.Id).ToList();
        var packBoxHistoryList = session.Query<LogisticsOrderPackBoxHistoryInfo>()
            .Where(e => packBoxIdList.Contains(e.PackBox.Id))
            .ToList();
        foreach (var packBoxHistory in packBoxHistoryList) {
            session.Delete(packBoxHistory);
        }

        // 删除打包箱号信息
        foreach (LogisticsOrderPackBoxInfo orderPackBox in orderPackBoxList) {
            session.Delete(orderPackBox);
        }

        // 删除打包信息
        session.Delete(orderPack);

        session.Flush();

        // 可能的删除 订单头程物流记录
        var orderFirstMode = orderPack.OrderFirstMode;
        int orderFirstModeId = orderFirstMode.Id;
        var orderPackHasOrderFirstMode = session.Query<LogisticsOrderPackInfo>()
            .Where(e => e.OrderFirstMode.Id == orderFirstModeId)
            .Take(1)
            .FirstOrDefault();

        // 订单打包关联的 订单头程物流记录 已经不再被引用了
        if (orderPackHasOrderFirstMode == null) {
            // 先删除关联的费用信息
            var firstModeOrderFeeList = session.Query<LogisticsOrderFeeInfo>()
                .Where(e => e.OrderFirstMode!.Id == orderFirstModeId)
                .ToList();
            foreach (var firstModeOrderFee in firstModeOrderFeeList) {
                session.Delete(firstModeOrderFee);
            }

            // 删除订单头程物流记录
            session.Delete(orderFirstMode);
            session.Flush();
        }

        // 如果当前状态是已打包 标记 物品，快递单号为 等待打包状态，同时订单也是
        UpdateRepackStatus(orderId, trackingNoList);

        // 快递单号日志
        int loginUserId = loginUserService.GetLoginUserId();
        foreach (ExpressInfo orderExpress in goodsTypeExpressList) {
            expressLogService.UserLog(orderExpress.Id, ExpressLogType.RemovePack, loginUserId);
        }

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "当前打包数据移除成功！" }
        });
    }

    [NonAction]
    private void UpdateRepackStatus(int orderId, IList<string> expressTrackingNoList) {
        if (expressTrackingNoList == null || expressTrackingNoList.Count == 0) {
            throw new ValidationException("快递单号列表不能为空！");
        }

        LogisticsOrderInfo logisticsOrder = session.Get<LogisticsOrderInfo>(orderId);
        if (logisticsOrder == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        if (logisticsOrder.OrderStatus == LogisticsOrderStatus.Packed) {
            logisticsOrder.OrderStatus = LogisticsOrderStatus.WaitPack;
            session.Update(logisticsOrder);
            session.Flush();
        }

        IList<ExpressInfo> expressList = session.Query<ExpressInfo>()
            .Where(e => e.Order!.Id == orderId && expressTrackingNoList.Contains(e.TrackingNo))
            .ToList();

        foreach (var express in expressList) {
            if (express.Status == ExpressStatus.Pack) {
                express.Status = ExpressStatus.WaitPack;
                session.Update(express);
            }
        }

        session.Flush();
    }

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/get/order/id/by/express/tracking/no")]
    public IActionResult GetOrderIdByExpressTrackingNo(string expressTrackingNo, int warehouseId) {
        var goodsExpress = session.Query<ExpressInfo>()
            .Fetch(e => e.Order)
            .Where(e => e.TrackingNo == expressTrackingNo)
            .Take(1)
            .SingleOrDefault();

        if (goodsExpress == null || goodsExpress.Order == null) {
            throw new ValidationException("本快递对应的订单不存在！");
        }

        if (goodsExpress.Order.OrderStatus != LogisticsOrderStatus.WaitPack) {
            throw new ValidationException("本快递对应的订单不是等待打包状态！");
        }

        if ((goodsExpress.Order.Warehouse.Id) != warehouseId) {
            WarehouseInfo warehouse = session.Get<WarehouseInfo>(warehouseId);
            if (warehouse == null) {
                throw new ValidationException($"指定的收货仓库 #{warehouseId} 不存在！");
            }

            throw new ValidationException($"本快递对应的订单的收货仓 {goodsExpress.Order.Warehouse.Name}，不是当前仓库 {warehouse.Name}！");
        }

        int goodsExpressOrderId = goodsExpress.Order?.Id ?? 0;

        if (loginUserService.IsAdmin == false) {
            var orderPackSchedule = session.Query<LogisticsOrderPackScheduleInfo>()
                .Fetch(e => e.User)
                .Where(e => e.Order.Id == goodsExpressOrderId)
                .Take(1)
                .SingleOrDefault();

            if (orderPackSchedule != null) {
                int loginUserId = loginUserService.GetLoginUserId();
                int orderPackScheduleUserId = orderPackSchedule.User?.Id ?? 0;

                if (orderPackScheduleUserId != loginUserId) {
                    throw new ValidationException($"快递单号所在订单已排班，但打包工为 {orderPackSchedule.User?.LoginName}({orderPackSchedule.User?.UserName})， 不是当前登录人！");
                }
            }
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "操作成功！" },
            { "orderId", goodsExpressOrderId }
        });
    }

    #region 完成打包

    [Obsolete]
    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/complete/goods/type/pack")]
    [Transactional]
    public IActionResult CompleteGoodsTypePack(LogisticsOrderGoodsPackCompleteGoodsTypePackParam param) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        int loginUserId = loginUserService.GetLoginUserId();

        int orderId = param.OrderId;
        LogisticsGoodsType goodsType = param.GoodsType;

        LogisticsOrderInfo logisticsOrder = session.Get<LogisticsOrderInfo>(orderId);
        if (logisticsOrder == null) {
            throw new ValidationException("指定的订单不存在！");
        }

        // 检查当前物品类型中的快递单号是不是都已经打包了
        var goodsTypeExpressNoQuery = session
            .Query<ExpressInfo>()
            .Where(e => e.Order!.Id == orderId);

        IList<string> goodsTypeExpressNoList = goodsTypeExpressNoQuery
            .Select(e => e.TrackingNo)
            .Distinct()
            .ToList();

        var packedGoodsTypeExpressNoQuery = session.Query<LogisticsOrderPackExpressInfo>()
            .Fetch(e => e.Express)
            .Where(e => e.Order.Id == orderId);

        IList<string> packedGoodsTypeExpressNoList = packedGoodsTypeExpressNoQuery
            .Select(e => e.Express.TrackingNo)
            .ToList();

        IList<string> notPackedGoodsTypeExpressNoList = goodsTypeExpressNoList.Where(e => packedGoodsTypeExpressNoList.Contains(e) == false).ToList();

        if (notPackedGoodsTypeExpressNoList.Count != 0) {
            throw new ValidationException($"订单当前物品类型中快递单号 {string.Join(",", notPackedGoodsTypeExpressNoList)} 未包含在任何箱号中，不能完成打包！");
        }

        // 检查当前物品类型有没有订单打包信息
        int orderPackCount = session.Query<LogisticsOrderPackInfo>().Where(e => e.Order.Id == orderId && e.GoodsType == goodsType).Count(); // 空运和快递订单时，这里也还是一样的，原来的不用改
        if (orderPackCount == 0) {
            throw new ValidationException("订单当前物品类型打包信息不存在，不能完成打包！");
        }

        // 更新快递号状态
        var orderGoodsExpressQuery = session
            .Query<ExpressInfo>()
            .Where(e => e.Order!.Id == orderId);

        IList<ExpressInfo> orderGoodsExpressList = orderGoodsExpressQuery.ToList();

        foreach (ExpressInfo orderGoodsExpress in orderGoodsExpressList) {
            orderGoodsExpress.Status = ExpressStatus.Pack;
            session.Update(orderGoodsExpress);
        }

        session.Flush();

        // 更新订单状态
        int notPackedExpressCount = session.Query<ExpressInfo>()
            .Where(e => e.Order!.Id == orderId && e.Status != ExpressStatus.Pack)
            .Count();

        // 是否订单完成打包
        bool orderPackComplete = notPackedExpressCount == 0;

        if (orderPackComplete) {
            logisticsOrderPackService.CompletePack(orderId, loginUserId);
        }

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "orderPackComplete", orderPackComplete },
            { "message", "完成订单当前物品类型打包成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/complete/order/pack")]
    [Transactional]
    public IActionResult CompleteOrderPack(LogisticsOrderGoodsPackCompleteOrderPackParam param) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        int loginUserId = loginUserService.GetLoginUserId();

        int orderId = param.OrderId;
        logisticsOrderPackService.CompletePack(orderId, loginUserId);

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "完成订单打包成功！" }
        });
    }

    #endregion

    #region 标签打印信息

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/get/print/pack/data/10/15")]
    [NoLoginRequired]
    public IActionResult GetPrintPackDataW10H15(int orderPackId) {
        var orderPackConfig = session
            .Query<LogisticsOrderPackConfigInfo>()
            .Take(1)
            .SingleOrDefault();

        if (orderPackConfig == null) {
            throw new Exception("打包配置还没配置，请先配置订单打包！");
        }

        int printCount = orderPackConfig.Label1015PrintCount;

        LogisticsOrderPackInfo orderPack = session.Get<LogisticsOrderPackInfo>(orderPackId);
        if (orderPack == null) {
            throw new ValidationException("指定的订单打包信息不存在！");
        }

        IList<LogisticsOrderPackBoxInfo> orderPackBoxList = session
            .Query<LogisticsOrderPackBoxInfo>()
            .Where(e => e.OrderPack.Id == orderPackId)
            .ToList();

        string customerCode = (orderPack.Order.Customer?.Id ?? 0).ToString();
        string orderCode = orderPack.Order.Code;

        IList<string> customerCodeList = new List<string>();
        IList<string> orderIdList = new List<string>();
        IList<string> orderCodeList = new List<string>();
        IList<string> orderPackBoxCodeList = new List<string>();
        IList<string> orderPackBoxShortCodeList = new List<string>();
        IList<int> orderPackBoxNumList = new List<int>();
        foreach (LogisticsOrderPackBoxInfo orderPackBox in orderPackBoxList) {
            customerCodeList.Add(customerCode);
            orderCodeList.Add(orderCode);
            orderIdList.Add(orderPackBox.Order.Id.ToString());

            orderPackBoxCodeList.Add(logisticsOrderPackBoxService.GetBoxCode(orderPackBox.Id));
            orderPackBoxShortCodeList.Add(logisticsOrderPackBoxService.GetBoxShortCode(orderPackBox.Id));
            orderPackBoxNumList.Add(orderPackBox.BoxNum);
        }

        int orderId = orderPack.Order.Id;
        string firstLogisticsTypeName = orderPack.Order.FirstMode.Name;

        string weight = orderPack.Weight.ToString("#0.0") + "Kg";
        string volumeWeight = (orderPack.Width * orderPack.Length * orderPack.Height / 5000m).ToString("#0.0") + "Kg";
        string volume = (orderPack.Width * orderPack.Length * orderPack.Height / 1000000m).ToString("#0.00") + "立方";
        string lwh = $"{orderPack.Length:#} x {orderPack.Width:#} x {orderPack.Height:#}CM";
        string userName = orderPack.PackUser.UserName;
        string packTime = orderPack.PackTime.ToString("MM-dd HH:mm");
        string expressGoodsTypeName = orderPack.GoodsType.Name;
        string orderGoodsTypeName = orderPack.Order.GoodsType.Name;

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "customerCode", customerCode },
            { "orderCode", orderCode }, // 这是原来的主订单编号
            { "orderId", orderId },
            { "firstLogisticsTypeName", firstLogisticsTypeName },
            { "weight", weight },
            { "volumeWeight", volumeWeight },
            { "volume", volume },
            { "lwh", lwh },
            { "customerCodeList", customerCodeList },
            { "orderIdList", orderIdList },
            { "orderCodeList", orderCodeList },
            { "orderPackBoxCodeList", orderPackBoxCodeList },
            { "orderPackBoxShortCodeList", orderPackBoxShortCodeList },
            { "orderPackBoxNumList", orderPackBoxNumList },
            { "userName", userName },
            { "packTime", packTime },
            { "orderGoodsTypeName", orderGoodsTypeName },
            { "expressGoodsTypeName", expressGoodsTypeName },
            { "printCount", printCount }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/get/print/pack/data/10/5")]
    [NoLoginRequired]
    public IActionResult GetPrintPackDataW10H5(int orderPackId) {
        var orderPackConfig = session
            .Query<LogisticsOrderPackConfigInfo>()
            .Take(1)
            .SingleOrDefault();

        if (orderPackConfig == null) {
            throw new Exception("打包配置还没配置，请先配置订单打包！");
        }

        int printCount = orderPackConfig.Label1005PrintCount;

        LogisticsOrderPackInfo orderPack = session.Get<LogisticsOrderPackInfo>(orderPackId);
        if (orderPack == null) {
            throw new ValidationException("指定的订单打包信息不存在！");
        }

        IList<LogisticsOrderPackBoxInfo> orderPackBoxList = session
            .Query<LogisticsOrderPackBoxInfo>()
            .Where(e => e.OrderPack.Id == orderPackId)
            .ToList();

        // string customerCode = orderPack.Order.CustomerCode;
        string customerCode = (orderPack.Order.Customer?.Id ?? 0).ToString();
        string orderCode = orderPack.Order.Code;

        IList<string> customerCodeList = new List<string>();
        IList<string> orderIdList = new List<string>();
        IList<string> orderCodeList = new List<string>();
        IList<string> orderPackBoxCodeList = new List<string>();
        IList<string> orderPackBoxShortCodeList = new List<string>();
        IList<int> orderPackBoxNumList = new List<int>();
        foreach (LogisticsOrderPackBoxInfo orderPackBox in orderPackBoxList) {
            customerCodeList.Add(customerCode);
            orderCodeList.Add(orderCode);
            orderIdList.Add(orderPackBox.Order.Id.ToString());

            orderPackBoxCodeList.Add(logisticsOrderPackBoxService.GetBoxCode(orderPackBox.Id));
            orderPackBoxShortCodeList.Add(logisticsOrderPackBoxService.GetBoxShortCode(orderPackBox.Id));
            orderPackBoxNumList.Add(orderPackBox.BoxNum);
        }

        string firstLogisticsTypeName = orderPack.Order.FirstMode.Name;

        string weight = orderPack.Weight.ToString("#0.0") + "Kg";
        string volumeWeight = (orderPack.Width * orderPack.Length * orderPack.Height / 5000m).ToString("#0.0") + "Kg";
        string volume = (orderPack.Width * orderPack.Length * orderPack.Height / 1000000m).ToString("#0.00") + "方";
        string userName = orderPack.PackUser.UserName;
        string packTime = orderPack.PackTime.ToString("MM-dd HH:mm");
        string expressGoodsTypeName = orderPack.GoodsType.Code;
        string orderGoodsTypeName = orderPack.Order.GoodsType.Code;

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "customerCode", customerCode },
            { "orderCode", orderCode },
            { "firstLogistics", firstLogisticsTypeName.Trim().FirstOrDefault() },
            { "weight", weight },
            { "volumeWeight", volumeWeight },
            { "volume", volume },
            { "customerCodeList", customerCodeList },
            { "orderIdList", orderIdList },
            { "orderCodeList", orderCodeList },
            { "orderPackBoxCodeList", orderPackBoxCodeList },
            { "orderPackBoxShortCodeList", orderPackBoxShortCodeList },
            { "orderPackBoxNumList", orderPackBoxNumList },
            { "userName", userName },
            { "packTime", packTime },
            { "orderGoodsTypeName", orderGoodsTypeName },
            { "expressGoodsTypeName", expressGoodsTypeName },
            { "printCount", printCount }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/get/print/pack/data/5/5")]
    [NoLoginRequired]
    public IActionResult GetPrintPackDataW5H5(int orderPackId) {
        var orderPackConfig = session
            .Query<LogisticsOrderPackConfigInfo>()
            .Take(1)
            .SingleOrDefault();

        if (orderPackConfig == null) {
            throw new Exception("打包配置还没配置，请先配置订单打包！");
        }

        int printCount = orderPackConfig.Label0505PrintCount;

        LogisticsOrderPackInfo orderPack = session.Get<LogisticsOrderPackInfo>(orderPackId);
        if (orderPack == null) {
            throw new ValidationException("指定的订单打包信息不存在！");
        }

        IList<LogisticsOrderPackBoxInfo> orderPackBoxList = session
            .Query<LogisticsOrderPackBoxInfo>()
            .Where(e => e.OrderPack.Id == orderPackId)
            .ToList();

        string customerCode = orderPack.Order.Customer.Id.ToString();
        string orderCode = orderPack.Order.Code;

        IList<string> customerCodeList = new List<string>();
        IList<string> orderIdList = new List<string>();
        IList<string> orderCodeList = new List<string>();
        IList<string> orderPackBoxCodeList = new List<string>();
        IList<string> orderPackBoxShortCodeList = new List<string>();
        IList<int> orderPackBoxNumList = new List<int>();
        foreach (LogisticsOrderPackBoxInfo orderPackBox in orderPackBoxList) {
            customerCodeList.Add(customerCode);
            orderCodeList.Add(orderCode);
            orderIdList.Add(orderPackBox.Order.Id.ToString());

            orderPackBoxCodeList.Add(logisticsOrderPackBoxService.GetBoxCode(orderPackBox.Id));
            orderPackBoxShortCodeList.Add(logisticsOrderPackBoxService.GetBoxShortCode(orderPackBox.Id));
            orderPackBoxNumList.Add(orderPackBox.BoxNum);
        }

        string firstLogisticsTypeName = orderPack.Order.FirstMode.Name;
        string weight = orderPack.Weight.ToString("#0.0") + "Kg";
        string volumeWeight = (orderPack.Width * orderPack.Length * orderPack.Height / 5000m).ToString("#0.0") + "Kg";
        string volume = (orderPack.Width * orderPack.Length * orderPack.Height / 1000000m).ToString("#0.00") + "方";
        string userName = orderPack.PackUser?.UserName ?? "";
        string packTime = orderPack.PackTime.ToString("MM-dd HH:mm");
        string expressGoodsTypeName = orderPack.GoodsType.Code;
        string orderGoodsTypeName = orderPack.Order.GoodsType.Code;

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "customerCode", customerCode },
            { "orderCode", orderCode },
            { "firstLogistics", firstLogisticsTypeName.Trim().FirstOrDefault() },
            { "weight", weight },
            { "volumeWeight", volumeWeight },
            { "volume", volume },
            { "customerCodeList", customerCodeList },
            { "orderIdList", orderIdList },
            { "orderCodeList", orderCodeList },
            { "orderPackBoxCodeList", orderPackBoxCodeList },
            { "orderPackBoxShortCodeList", orderPackBoxShortCodeList },
            { "orderPackBoxNumList", orderPackBoxNumList },
            { "userName", userName },
            { "packTime", packTime },
            { "orderGoodsTypeName", orderGoodsTypeName },
            { "expressGoodsTypeName", expressGoodsTypeName },
            { "printCount", printCount }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/get/print/pack/data/air/express")]
    [NoLoginRequired]
    public IActionResult GetPrintPackDataAirExpress(int orderPackId) {
        int printCount = 1;

        LogisticsOrderPackInfo orderPack = session.Get<LogisticsOrderPackInfo>(orderPackId);
        if (orderPack == null) {
            throw new ValidationException("指定的订单打包信息不存在！");
        }

        LogisticsOrderInfo order = orderPack.Order;
        int orderId = order.Id;

        var receiveAddress = session.Query<LogisticsOrderReceiveAddressInfo>()
            .Where(e => e.Order.Id == orderId)
            .Take(1)
            .SingleOrDefault();
        if (receiveAddress == null) {
            throw new ValidationException($"订单#{orderId}收货地址不存在，数据存在问题！");
        }

        string customerCode = order.Customer.Id.ToString();
        string orderCode = order.Code;

        string firstLogisticsModeName = order.FirstMode.Name;
        string actualWeight = orderPack.Weight.ToString("#0.0") + "Kg";
        string volumeWeight = (orderPack.Width * orderPack.Length * orderPack.Height / 5000m).ToString("#0.0") + "Kg";
        string volume = (orderPack.Width * orderPack.Length * orderPack.Height / 1000000m).ToString("#0.00") + "立方";
        string userName = orderPack.PackUser?.UserName ?? "";
        string packTime = orderPack.PackTime.ToString("MM-dd HH:mm");
        string consignee = receiveAddress.FirstName + " " + receiveAddress.LastName;
        string mobile = receiveAddress.Mobile;
        string address = $"{receiveAddress.Address}, {receiveAddress.City} {receiveAddress.State} {receiveAddress.Zipcode}";
        if (string.IsNullOrWhiteSpace(receiveAddress.UnitLoc) == false) {
            address = $"{receiveAddress.UnitLoc}, " + address;
        }

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "customerCode", customerCode },
            { "orderCode", orderCode },
            { "orderId", orderId },
            { "firstLogisticsModeName", firstLogisticsModeName },
            { "actualWeight", actualWeight },
            { "volumeWeight", volumeWeight },
            { "volume", volume },
            { "userName", userName },
            { "packTime", packTime },
            { "consignee", consignee },
            { "mobile", mobile },
            { "address", address },
            { "printCount", printCount }
        });
    }

    #endregion
}
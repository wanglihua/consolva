using System.Text;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using shunda.Service.Express;
using shunda.Service.Logistics;
using shunda.Service.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.ReceivePack.Controllers;

[ReceivePackArea]
[AdminAuth(PassRes.ReceivePack.OrderPack)]
public class LogisticsOrderGoodsPackController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly LogisticsOrderPackBoxService logisticsOrderPackBoxService;
    private readonly ExpressTrackingNoCorrectService expressTrackingNoCorrectService;
    private readonly LogisticsOrderPackConfigService logisticsOrderPackConfigService;
    private readonly LogisticsOrderPackLabelTypeService logisticsOrderPackLabelTypeService;
    private readonly LogisticsOrderPackBoxHistoryService logisticsOrderPackBoxHistoryService;
    private readonly LogisticsModeService logisticsModeService;
    private readonly ExpressLogService expressLogService;
    private readonly LogisticsOrderService logisticsOrderService;
    private readonly WarehouseService warehouseService;

    public LogisticsOrderGoodsPackController(ISession session,
        LoginUserService loginUserService,
        LogisticsOrderPackBoxService logisticsOrderPackBoxService,
        ExpressTrackingNoCorrectService expressTrackingNoCorrectService,
        LogisticsOrderPackConfigService logisticsOrderPackConfigService,
        LogisticsOrderPackLabelTypeService logisticsOrderPackLabelTypeService,
        LogisticsOrderPackBoxHistoryService logisticsOrderPackBoxHistoryService,
        LogisticsModeService logisticsModeService,
        ExpressLogService expressLogService,
        LogisticsOrderService logisticsOrderService,
        WarehouseService warehouseService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.logisticsOrderPackBoxService = logisticsOrderPackBoxService;
        this.expressTrackingNoCorrectService = expressTrackingNoCorrectService;
        this.logisticsOrderPackConfigService = logisticsOrderPackConfigService;
        this.logisticsOrderPackLabelTypeService = logisticsOrderPackLabelTypeService;
        this.logisticsOrderPackBoxHistoryService = logisticsOrderPackBoxHistoryService;
        this.logisticsModeService = logisticsModeService;
        this.expressLogService = expressLogService;
        this.logisticsOrderService = logisticsOrderService;
        this.warehouseService = warehouseService;
    }

    [HttpGet]
    [Route("/admin/logistics/order/goods/pack/{warehouseId:int}")]
    public IActionResult Index(int warehouseId) {
        WarehouseInfo warehouse = session.Get<WarehouseInfo>(warehouseId);
        if (warehouse == null) {
            throw new ValidationException("指定的仓库不存在！");
        }

        int loginUserId = loginUserService.GetLoginUserId();

        LogisticsOrderGoodsPackIndexModel model = new LogisticsOrderGoodsPackIndexModel();
        model.Warehouse = warehouse;

        var printTypeList = logisticsOrderPackConfigService.GetPrintTypeList();
        model.PrintTypeList = printTypeList;

        LogisticsOrderPackPrintType printType = logisticsOrderPackConfigService.GetUserPackPrintType(loginUserId, warehouseId);

        model.PrintTypeSelected = LogisticsOrderPackPrintType.None;
        if (printType != null && printType != LogisticsOrderPackPrintType.None) {
            model.PrintTypeSelected = printType;
        }

        // 把这个页面最后使用的打印方式保存一下，后来感觉这个保存也不需要了
        // logisticsOrderPackConfigService.SaveUserProfile(loginUserId, warehouseId, printType);     

        return View("LogisticsOrderGoodsPackIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/save/print/type")]
    public IActionResult SavePrintType(int warehouseId, int printTypeId) {
        int loginUserId = loginUserService.GetLoginUserId();
        logisticsOrderPackConfigService.SaveUserProfile(loginUserId, warehouseId, LogisticsOrderPackPrintType.GetById(printTypeId));

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "操作成功！" }
        });
    }

    /// <summary>
    /// 这个是选择订单的对话框
    /// </summary>
    /// <param name="warehouseId"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/list/dialog")]
    public IActionResult ListDialog(int warehouseId) {
        WarehouseInfo warehouse = session.Get<WarehouseInfo>(warehouseId);
        if (warehouse == null) {
            throw new ValidationException("指定的仓库不存在！");
        }

        IList<LogisticsOrderStatus> orderStatusList = new List<LogisticsOrderStatus>();
        orderStatusList.Add(LogisticsOrderStatus.WaitPack);
        orderStatusList.Add(LogisticsOrderStatus.Packed);

        LogisticsOrderGoodsPackListDialogModel model = new LogisticsOrderGoodsPackListDialogModel();
        model.Warehouse = warehouse;
        model.OrderStatusList = orderStatusList;

        var firstLogisticsModeTypeList = LogisticsModeType.GetList();
        model.FirstLogisticsModeTypeList = firstLogisticsModeTypeList;

        model.FirstLogisticsModeList = session.Query<LogisticsModeInfo>()
            .Where(e => e.Cate == LogisticsModeCate.Customer
                        && firstLogisticsModeTypeList.Contains(e.Type)
                        && e.InnerStatus != LogisticsModeStatus.Disabled)
            .OrderBy(e => e.Name)
            .ToList();

        model.ReceivingWarehouseList = warehouseService.GetWarehouseListForAdmin();

        model.GoodsTypeList = LogisticsGoodsType.GetCustomerList();

        return View("LogisticsOrderGoodsPackListDialog", model);
    }

    [Obsolete("换成按国家选择物流方式")]
    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/list/first/logistics/mode/select")]
    public IActionResult ListQueryFirstLogisticsModeSelect(int targetWarehouseId) {
        LogisticsOrderGoodsPackListDialogModel model = new LogisticsOrderGoodsPackListDialogModel();

        var firstLogisticsModeTypeList = LogisticsModeType.GetList();

        var query = session.Query<LogisticsModeInfo>()
            .Where(e => e.Cate == LogisticsModeCate.Customer
                        && firstLogisticsModeTypeList.Contains(e.Type)
                        && e.InnerStatus != LogisticsModeStatus.Disabled);

        if (targetWarehouseId != 0) {
            query = query.Where(e => e.Warehouse.Id == targetWarehouseId);
        }

        model.FirstLogisticsModeList = query
            .OrderBy(e => e.Name)
            .ToList();

        return View("LogisticsOrderGoodsPackListQueryFirstLogisticsModeSelect", model);
    }

    /// <summary>
    /// 这个是选择订单的对话框里面的列表数据
    /// </summary>
    /// <param name="queryParam"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/list")]
    public IActionResult List(LogisticsOrderGoodsPackListQueryParam queryParam) {
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

            order.PackScheduleUserName = orderPackSchedule?.User?.LoginName;
        }

        LogisticsOrderGoodsPackListModel model = new LogisticsOrderGoodsPackListModel(listPager);
        model.OrderList = orderList;

        return View("LogisticsOrderGoodsPackList", model);
    }

    /// <summary>
    /// 这里的查询条件, 显示当前打包订单的时候使用, 显示对话框订单列表的时候也使用的
    /// </summary>
    /// <param name="queryParam"></param>
    /// <returns></returns>
    [NonAction]
    private ICriteria GetListCriteria(LogisticsOrderGoodsPackListQueryParam queryParam) {
        LogisticsOrderPackDisplayType displayType = LogisticsOrderPackDisplayType.NotOthers;
        LogisticsOrderPackConfigInfo logisticsOrderPackConfig = session
            .Query<LogisticsOrderPackConfigInfo>()
            .Take(1)
            .SingleOrDefault();

        if (logisticsOrderPackConfig != null) {
            if (logisticsOrderPackConfig.DisplayType == LogisticsOrderPackDisplayType.None) {
                displayType = LogisticsOrderPackDisplayType.NotOthers;
            } else {
                displayType = logisticsOrderPackConfig.DisplayType;
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
            DateTime fifteenMinutesBefore = DateTime.Now.AddMinutes(-15);
            criteria = criteria.Add(Restrictions.Or(
                Restrictions.IsNotNull($"{nameof(LogisticsOrderInfo.PackSchedules)}.{nameof(LogisticsOrderPackScheduleInfo.User)}"), // 已排班的订单不受这个时间限制
                Restrictions.Where<LogisticsOrderInfo>(e => e.SubmitPackTime <= fifteenMinutesBefore)
            ));
        }

        // 先显示排班的订单
        // criteria = criteria.AddOrder(new Order($"{nameof(LogisticsOrderInfo.PackSchedules)}.{nameof(LogisticsOrderPackScheduleInfo.User)}", false));

        #region 页面其他手工查询条件

        // 订单编号查询
        string orderCodeQuery = queryParam.OrderCodeQuery;
        if (string.IsNullOrWhiteSpace(orderCodeQuery) == false) {
            criteria = criteria.Add(Restrictions.On<LogisticsOrderInfo>(e => e.Code).IsInsensitiveLike(orderCodeQuery, MatchMode.Anywhere));
        }

        int firstLogisticsModeIdQuery = queryParam.FirstLogisticsModeIdQuery;
        if (firstLogisticsModeIdQuery != 0) {
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.FirstMode.Id == firstLogisticsModeIdQuery));
        }

        LogisticsGoodsType goodsTypeQuery = queryParam.GoodsTypeQuery;
        if (goodsTypeQuery != null && goodsTypeQuery != LogisticsGoodsType.None) {
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
    /// <param name="queryParam"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/order")]
    public IActionResult Order(LogisticsOrderGoodsPackOrderQueryParam queryParam) {
        int? nextOrderId = queryParam.NextOrderId;
        int? preOrderId = queryParam.OrderId;
        int warehouseId = queryParam.WarehouseId;

        LogisticsOrderGoodsPackOrderModel model = new LogisticsOrderGoodsPackOrderModel();
        model.TrackingNoManualInputEnabled = true;

        // 就是这个URL对应界面打开后，当前打包的这个订单的ID
        int nextOrderIdValue = 0;
        if (nextOrderId.HasValue) {
            nextOrderIdValue = nextOrderId.Value;
        } else {
            LogisticsOrderPackSortType sortType = LogisticsOrderPackSortType.StorageLoc;
            LogisticsOrderPackDisplayType displayType = LogisticsOrderPackDisplayType.NotOthers;

            LogisticsOrderPackConfigInfo logisticsOrderPackConfig = session
                .Query<LogisticsOrderPackConfigInfo>()
                .Take(1)
                .SingleOrDefault();

            if (logisticsOrderPackConfig != null) {
                sortType = logisticsOrderPackConfig.SortType;

                if (logisticsOrderPackConfig.DisplayType == LogisticsOrderPackDisplayType.None) {
                    displayType = LogisticsOrderPackDisplayType.NotOthers;
                } else {
                    displayType = logisticsOrderPackConfig.DisplayType;
                }

                model.TrackingNoManualInputEnabled = logisticsOrderPackConfig.TrackingNoManualInputEnabled;
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
            int loginUserId = loginUserService.GetLoginUserId();
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

        LogisticsOrderInfo logisticsOrder = session.Get<LogisticsOrderInfo>(nextOrderIdValue);

        model.IsAdmin = loginUserService.IsAdmin;

        model.Order = logisticsOrder;

        if (logisticsOrder != null) {
            IList<LogisticsOrderGoodsExpressInfo> orderGoodsExpressList = session.Query<LogisticsOrderGoodsExpressInfo>().Where(e => e.Order.Id == nextOrderIdValue).ToList();
            IList<LogisticsOrderGoodsInfo> orderGoodsList = session.Query<LogisticsOrderGoodsInfo>()
                .Fetch(e => e.Express)
                .Fetch(e => e.StorageLoc)
                .Where(e => e.Express.Order.Id == nextOrderIdValue)
                .ToList();
            foreach (LogisticsOrderGoodsExpressInfo orderGoodsExpress in orderGoodsExpressList) {
                IList<LogisticsOrderGoodsInfo> expressGoodsList = orderGoodsList.Where(e => e.Express.Id == orderGoodsExpress.Id).ToList();

                IList<string> storageLocNameList = expressGoodsList
                    .OrderBy(e => e.StorageLoc?.Shelf?.OrderNum)
                    .ThenBy(e => e.StorageLoc?.RowNum)
                    .ThenBy(e => e.StorageLoc?.ColNum)
                    .Select(e => e.StorageLoc?.GetFullName())
                    .Distinct()
                    .ToList();
                orderGoodsExpress.StorageLocNames = string.Join("， ", storageLocNameList);

                IList<int> goodsAdminIdList = expressGoodsList.Where(e => e.GoodsAdminId != null).Select(e => e.GoodsAdminId.Value).Distinct().ToList();

                IList<string> goodsAdminNameList = session.Query<LogisticsGoodsInfo>()
                    .Where(e => goodsAdminIdList.Contains(e.Id))
                    .ToList()
                    .Select(e => e.GetGoodsAdminName())
                    .ToList();
                orderGoodsExpress.GoodsAdminNames = string.Join(", ", goodsAdminNameList);
            }


            model.GoodsTypeList = LogisticsGoodsType.GetInnerList(); // 这是使用 GetCustomerList() 也可以，但用 Inner 也无妨，万一订单中有不符合要求的物品


            model.GoodsTypePackCompleteDict = new Dictionary<LogisticsGoodsType, bool>();
            foreach (LogisticsGoodsType goodsType in model.GoodsTypeList) {
                int goodsTypeNotPackedCount = orderGoodsExpressList.Where(e => e.GoodsType == goodsType && e.Status != LogisticsOrderGoodsExpressStatus.Packed).Count();
                if (goodsTypeNotPackedCount == 0) {
                    model.GoodsTypePackCompleteDict[goodsType] = true;
                } else {
                    model.GoodsTypePackCompleteDict[goodsType] = false;
                }
            }

            // 按仓位号排序，使用的是内存中排序的方式
            orderGoodsExpressList = orderGoodsExpressList.OrderBy(e => e.StorageLocNames).ToList();

            // 获取自动测量的数据，现在是老的建单方式，还不能通过订单ID的方式进行引用
            IList<string> trackingNoList = orderGoodsExpressList.Select(e => e.ExpressTrackingNo).Distinct().ToList();
            IList<LogisticsExpressInfo> logisticsExpressList = session.Query<LogisticsExpressInfo>().Where(e => trackingNoList.Contains(e.TrackingNo)).ToList();
            IList<int> logisticsExpressIdList = logisticsExpressList.Select(e => e.Id).ToList();
            IList<LogisticsExpressChildInfo> logisticsExpressChildList = session.Query<LogisticsExpressChildInfo>()
                .Where(e => logisticsExpressIdList.Contains(e.Parent.Id))
                .OrderBy(e => e.Id)
                .ToList();

            IList<LogisticsClaimGoodsInfo> claimGoodsList = session.Query<LogisticsClaimGoodsInfo>().Where(e => trackingNoList.Contains(e.TrackingNo)).ToList();

            foreach (LogisticsOrderGoodsExpressInfo goodsExpress in orderGoodsExpressList) {
                LogisticsExpressInfo logisticsExpress = logisticsExpressList.Where(e => e.TrackingNo == goodsExpress.ExpressTrackingNo).Take(1).SingleOrDefault();
                if (logisticsExpress != null) {
                    goodsExpress.LengthCache = logisticsExpress.Length ?? 0;
                    goodsExpress.WidthCache = logisticsExpress.Width ?? 0;
                    goodsExpress.HeightCache = logisticsExpress.Height ?? 0;
                    goodsExpress.WeightCache = logisticsExpress.Weight ?? 0;

                    goodsExpress.HasSizeWeightDataCache = true;

                    int logisticsExpressId = logisticsExpress.Id;
                    goodsExpress.ExpressChildListCache = logisticsExpressChildList
                        .Where(e => e.Parent.Id == logisticsExpressId)
                        .OrderBy(e => e.Id)
                        .ToList();
                }

                goodsExpress.ClaimGoodsCache = claimGoodsList.Where(e => e.TrackingNo == goodsExpress.ExpressTrackingNo).Take(1).SingleOrDefault();
            }

            model.ExpressList = orderGoodsExpressList;

            IList<string> expressTrackingNoList = orderGoodsExpressList.Select(e => e.ExpressTrackingNo).ToList();

            model.OrderTotalPackageCount =
                orderGoodsExpressList
                    .Where(e => e.CheckinPackageCount.HasValue).Count() == 0
                    ? 0
                    : orderGoodsExpressList
                        .Where(e => e.CheckinPackageCount.HasValue)
                        .Sum(e => e.CheckinPackageCount.Value);

            // Common
            model.GoodsTypeCommonPackageCount =
                orderGoodsExpressList
                    .Where(e => e.GoodsType == LogisticsGoodsType.Common && e.CheckinPackageCount.HasValue).Count() == 0
                    ? 0
                    : orderGoodsExpressList
                        .Where(e => e.GoodsType == LogisticsGoodsType.Common && e.CheckinPackageCount.HasValue)
                        .Sum(e => e.CheckinPackageCount.Value);

            model.LogisticsOrderRemarkList = session.Query<LogisticsOrderRemarkInfo>()
                .Where(e => e.Order.Id == nextOrderIdValue)
                .OrderBy(e => e.Express.Id)
                .ThenBy(e => e.Id)
                .ToList();

            model.IsOrderPackComplete = logisticsOrder.OrderStatus == LogisticsOrderStatus.Packed;

            {
                LogisticsOrderPackScheduleInfo orderPackSchedule = session.Query<LogisticsOrderPackScheduleInfo>()
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

        string oldExpressTrackingNo = expressTrackingNo;
        expressTrackingNo = expressTrackingNoCorrectService.Correct(expressTrackingNo);

        LogisticsGoodsType goodsType = LogisticsGoodsType.GetById(goodsTypeId);

        /*
            已成功查找到快递号并选择！本快递单号入库包裹数量为 1，请确保下架包裹数量正确！
            未能找到录入的快递单号，请核对
         */

        LogisticsOrderGoodsExpressInfo orderGoodsExpress = session
            .Query<LogisticsOrderGoodsExpressInfo>()
            .Fetch(e => e.Order)
            .Where(e => e.Order.Id == orderId && e.ExpressTrackingNo == expressTrackingNo)
            .OrderBy(e => e.Status)
            .Take(1)
            .SingleOrDefault();

        // 如果订单快递信息中没找到快递单号，就尝试用澳拼入库号来匹配快递单号
        bool isAoPinTrackingNo = false;
        if (orderGoodsExpress == null) {
            string aoPinTrackingNo = expressTrackingNo;
            AoPinTrackingNoExpressInfo aoPinTrackingNoExpress = session.Query<AoPinTrackingNoExpressInfo>()
                .Where(e => e.AoPinTrackingNo == aoPinTrackingNo)
                .Take(1)
                .SingleOrDefault();

            if (aoPinTrackingNoExpress != null) {
                expressTrackingNo = aoPinTrackingNoExpress.TrackingNo;
                orderGoodsExpress = session
                    .Query<LogisticsOrderGoodsExpressInfo>()
                    .Fetch(e => e.Order)
                    .Where(e => e.Order.Id == orderId && e.ExpressTrackingNo == expressTrackingNo)
                    .OrderBy(e => e.Status)
                    .Take(1)
                    .SingleOrDefault();

                if (orderGoodsExpress != null) {
                    isAoPinTrackingNo = true;
                }
            }
        }

        if (orderGoodsExpress == null) {
            return Json(new Dictionary<string, object> {
                { "success", false },
                { "expressInOrder", false },
                { "message", "未能在当前订单中找到录入的快递单号，请核对！" }
            });
        }

        // 如果是空运和快递订单的，合并快递单号的物品类型，取用订单的物品类型
        LogisticsOrderInfo order = orderGoodsExpress.Order;
        if (order.FirstModeType != LogisticsModeType.CnAuAir && order.FirstModeType != LogisticsModeType.CnAuExpress) {
            if (orderGoodsExpress.GoodsType != goodsType) {
                return Json(new Dictionary<string, object> {
                    { "success", false },
                    { "expressInOrder", true },
                    { "message", "在当前订单中找到录入的快递单号，但快递单号物品类型跟当前不符合，请核对！" }
                });
            }
        }

        string message = $"{expressTrackingNo} 已成功查找到快递号并选择！本快递单号入库包裹数量为 {orderGoodsExpress.CheckinPackageCount}，请确保下架包裹数量正确！";

        if (oldExpressTrackingNo != expressTrackingNo) {
            if (isAoPinTrackingNo == false) {
                message = $"{expressTrackingNo} 已成功查找到快递号并选择！本快递单号入库包裹数量为 {orderGoodsExpress.CheckinPackageCount}，请确保下架包裹数量正确！本快递单号已经过更正，现为{expressTrackingNo}，原为{oldExpressTrackingNo}";
            } else {
                message = $"{expressTrackingNo} 已成功查找到快递号并选择！本快递单号入库包裹数量为 {orderGoodsExpress.CheckinPackageCount}，请确保下架包裹数量正确！扫描单号为澳拼入库号，澳拼入库号为{oldExpressTrackingNo}，对应快递单号为{expressTrackingNo}";
            }
        }

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "packageCount", orderGoodsExpress.CheckinPackageCount },
            { "expressId", orderGoodsExpress.Id },
            { "expressTrackingNo", orderGoodsExpress.ExpressTrackingNo },
            { "message", message }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/order/pack/list")]
    public IActionResult OrderPackList(int orderId, int goodsTypeId, bool repack) {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);

        LogisticsGoodsType goodsType = LogisticsGoodsType.GetById(goodsTypeId);

        var orderPackQuery = session
            .Query<LogisticsOrderPackInfo>()
            .Where(e => e.Order.Id == orderId);

        if (order.FirstModeType != LogisticsModeType.CnAuAir && order.FirstModeType != LogisticsModeType.CnAuExpress) {
            orderPackQuery = orderPackQuery.Where(e => e.GoodsType == goodsType);
        }

        IList<LogisticsOrderPackInfo> orderPackList = orderPackQuery.ToList();

        LogisticsOrderGoodsPackOrderPackListModel model = new LogisticsOrderGoodsPackOrderPackListModel();

        // 判断该类型是不是都是打包状态
        var goodsTypeNotPackedCountQuery = session.Query<LogisticsOrderGoodsExpressInfo>()
            .Where(e => e.Order.Id == orderId && e.Status != LogisticsOrderGoodsExpressStatus.Packed);

        if (order.FirstModeType != LogisticsModeType.CnAuAir && order.FirstModeType != LogisticsModeType.CnAuExpress) {
            goodsTypeNotPackedCountQuery = goodsTypeNotPackedCountQuery.Where(e => e.GoodsType == goodsType);
        }

        int goodsTypeNotPackedCount = goodsTypeNotPackedCountQuery.Count();

        if (goodsTypeNotPackedCount == 0) {
            model.GoodsTypePackComplete = true;
        } else {
            model.GoodsTypePackComplete = false;
        }

        model.GoodsType = goodsType;
        model.OrderPackList = orderPackList;
        model.Repack = repack;

        LogisticsOrderPackInfo orderPackLast = orderPackList
            .OrderByDescending(e => e.CreateTime)
            .Take(1)
            .SingleOrDefault();

        if (orderPackLast != null) {
            model.MultiExpressOnePackLastPack = orderPackLast.MultiExpressOnePack;
        } else {
            model.MultiExpressOnePackLastPack = null;
        }

        return View("LogisticsOrderGoodsPackOrderPackList", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/use/size/weight")]
    public IActionResult UseSizeWeight(LogisticsOrderGoodsPackUseSizeWeightParam param) {
        if (param.ExpressTrackingNoList == null || param.ExpressTrackingNoList.Count == 0) {
            throw new ValidationException("请先扫描要打包的快递单号！");
        }

        IList<string> expressTrackingNoList = param.ExpressTrackingNoList;

        IList<LogisticsExpressInfo> expressList = session.Query<LogisticsExpressInfo>()
            .Where(e => expressTrackingNoList.Contains(e.TrackingNo))
            .ToList();

        // if (expressList.Count == 0)
        // {
        //     throw new ValidationException("扫描的快递单号，没有找到对应的尺寸重量数据！");
        // }

        foreach (string expressTrackingNo in expressTrackingNoList) {
            LogisticsExpressInfo logisticsExpress = expressList.Where(e => e.TrackingNo == expressTrackingNo).Take(1).SingleOrDefault();
            if (logisticsExpress == null) {
                throw new ValidationException($"快递单号 {expressTrackingNo}，没能找到对应的尺寸重量数据！");
            }
        }

        decimal length = expressList[0].Length ?? 0;
        decimal width = expressList[0].Width ?? 0;
        decimal height = expressList[0].Height ?? 0;
        decimal weight = expressList[0].Weight ?? 0;

        if (expressList.Count() > 1) {
            decimal totalVolume = 0m; // 立方厘米
            decimal totalWeight = 0m; // kg

            foreach (LogisticsExpressInfo express in expressList) {
                totalVolume += (express.Length ?? 0) * (express.Width ?? 0) * (express.Height ?? 0) * (express.PackageCount ?? 1);
                totalWeight += express.Weight ?? 0;
            }

            IList<int> expressIdList = expressList.Select(e => e.Id).ToList();
            IList<LogisticsExpressChildInfo> expressChildList = session.Query<LogisticsExpressChildInfo>()
                .Where(e => expressIdList.Contains(e.Parent.Id))
                .ToList();

            foreach (LogisticsExpressChildInfo expressChild in expressChildList) {
                totalVolume += (expressChild.Length ?? 0) * (expressChild.Width ?? 0) * (expressChild.Height ?? 0) * (expressChild.PackageCount ?? 1);
                totalWeight += (expressChild.Weight ?? 0) * (expressChild.PackageCount ?? 1);
            }

            decimal sideLength = (decimal)Math.Pow((double)totalVolume, 0.333333333d);
            length = sideLength;
            width = sideLength;
            height = sideLength;

            weight = totalWeight;
        } else {
            int expressId = expressList[0].Id;
            IList<LogisticsExpressChildInfo> expressChildList = session.Query<LogisticsExpressChildInfo>()
                .Where(e => e.Parent.Id == expressId)
                .ToList();

            if (expressChildList.Count > 0) {
                decimal totalVolume = length * width * height; // 立方厘米
                decimal totalWeight = weight; // kg

                foreach (LogisticsExpressChildInfo expressChild in expressChildList) {
                    totalVolume += (expressChild.Length ?? 0) * (expressChild.Width ?? 0) * (expressChild.Height ?? 0) * (expressChild.PackageCount ?? 1);
                    totalWeight += (expressChild.Weight ?? 0) * (expressChild.PackageCount ?? 1);
                }

                decimal sideLength = (decimal)Math.Pow((double)totalVolume, 0.333333333d);
                length = sideLength;
                width = sideLength;
                height = sideLength;

                weight = totalWeight;
            }
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "操作成功！" },
            { "length", decimal.Round(length, 2, MidpointRounding.AwayFromZero) },
            { "width", decimal.Round(width, 2, MidpointRounding.AwayFromZero) },
            { "height", decimal.Round(height, 2, MidpointRounding.AwayFromZero) },
            { "weight", decimal.Round(weight, 3, MidpointRounding.AwayFromZero) },
        });
    }

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/save/pack")]
    public IActionResult SavePack(LogisticsOrderGoodsPackSavePackParam param) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        if (param.Width <= 0 || param.Length <= 0 || param.Height <= 0 || param.Weight <= 0) {
            throw new ValidationException($"长宽高重量数据不能小于0！");
        }

        int orderId = param.OrderId;

        int loginUserId = loginUserService.GetLoginUserId();

        UserInfo loginUser = session.Get<UserInfo>(loginUserId);

        IList<string> expressTrackingNoList = param.ExpressTrackingNoList;
        if (expressTrackingNoList == null || expressTrackingNoList.Count == 0) {
            throw new ValidationException("请扫描或输入快递单号，再保存打包信息！");
        }

        IList<LogisticsClaimGoodsInfo> claimGoodsList = session.Query<LogisticsClaimGoodsInfo>().Where(e => expressTrackingNoList.Contains(e.TrackingNo)).ToList();

        bool woodFrameNotDone = claimGoodsList.Where(e => e.WoodFrameFinal == true && (e.WoodFrameDone == null || e.WoodFrameDone == false)).Count() != 0;
        if (woodFrameNotDone) {
            throw new ValidationException("当前快递单号有未完成的打木架服务，请先完成打木架再保存打包！");
        }

        bool barkCleanNotDone = claimGoodsList.Where(e => e.BarkCleanFinal == true && (e.BarkCleanDone == null || e.BarkCleanDone == false)).Count() != 0;
        if (barkCleanNotDone) {
            throw new ValidationException("当前快递单号有未完成的清理树皮杂物服务，请先完成清理树皮杂物再保存打包！");
        }

        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException("指定的订单不存在！");
        }

        // 一次性获取订单的所有快递单备用
        var orderGoodsExpressList = session.Query<LogisticsOrderGoodsExpressInfo>()
            .Where(e => e.Order.Id == orderId)
            .ToList();

        IList<string> orderTrackingNoList = orderGoodsExpressList.Select(e => e.ExpressTrackingNo).ToList();
        IList<LogisticsExpressItem> expressSizeWeightList = logisticsExpressService.GetLogisticsExpressItemList(orderTrackingNoList);

        decimal orderExpressWeightTotal = 0m;
        if (expressSizeWeightList.Count > 0) {
            orderExpressWeightTotal = expressSizeWeightList.Sum(e => e.SingleWeight * e.Quantity);
        }

        // 获取分拆配置
        LogisticsOrderPackSplitSettingInfo splitSetting = logisticsOrderPackSplitSettingService.GetSetting();

        // 一次性获取订单所有打包分拆备用
        var orderPackSplitList = new List<LogisticsOrderPackSplitInfo>();
        if (splitSetting.Status == LogisticsOrderPackSplitSettingStatus.Enabled) {
            if (expressTrackingNoList.Count >= 2 && param.Weight >= splitSetting.SinglePackLimitWeight) {
                throw new ValidationException($"订单打包分拆已启用，多个快递单号打包时重量不能超过当前配置的起始单箱重量 {splitSetting.SinglePackLimitWeight} KG！");
            }

            orderPackSplitList = session.Query<LogisticsOrderPackSplitInfo>()
                .Where(e => e.OrderId == orderId)
                .ToList();

            // 添加 起始单箱重量 分拆
            // 这个方法调用后，如果有新增的 split，这个新增的 split 也会被添加的 给的参数 orderPackSplitList 里面的
            logisticsOrderPackSplitService.AddSinglePackWeightLimitSplitIfNeed(order, expressSizeWeightList, orderPackSplitList, splitSetting);
        }

        /*
         有些自己寄的物品，有好多物品，但也只有一个快递单号，这时这个也应该算是合箱的，所以这个判断是不对的。
        expressTrackingNoList = expressTrackingNoList.Distinct().ToList();
        if (expressTrackingNoList.Count == 1 && param.MultiExpressOnePack == true)
        {
            throw new ValidationException("只扫描了一个快递单号，却人工选择了合箱，两者有冲突！");
        }
        */

        // 回写申领件状态，这个操作也跟3.0的操作可以兼容
        session.Query<LogisticsClaimGoodsInfo>()
            .Where(e => expressTrackingNoList.Contains(e.TrackingNo))
            .UpdateBuilder()
            .Set(e => e.Picked, true)
            .Set(e => e.WarehouseProcessed, true)
            .Set(e => e.ShelfStatus, ExpressShelfStatus.OffShelf) // 设置为下架状态，不管前面有没有做过下架操作
            .Set(e => e.Dumped, false) // 设置为取消堆存，不管之前有没有过堆存
            .Set(e => e.Status, LogisticsClaimGoodsStatus.Pack)
            .Set(e => e.UpdateTime, DateTime.Now)
            .Update();

        // 如果是空运和快递订单的，合并快递单号的物品类型，取用订单的物品类型
        if (order.FirstModeType == LogisticsModeType.CnAuAir || order.FirstModeType == LogisticsModeType.CnAuExpress) {
            param.GoodsType = order.GoodsType; // 这个语句感觉不写也可以的，界面上传递的时候已经是订单的物品类型了
        }

        if (logisticsModeService.IsOrderExpressWeightFitMode(orderId, (order.FirstMode?.Id ?? 0), FeeMessageType.User) == false) {
            throw new ValidationException($"当前订单#{orderId}快递单号实际重或体积重跟订单选定的头程物流方式#{(order.FirstMode?.Id ?? 0)}重量限制不匹配，请重新更改为正确头程物流方式后再打包！");
        }

        bool multiExpressOnePack = false; // 这个值现在计算好，后面会设置到打包属性中

        // 现在这个值已经不传了，所有总是 null
        if (param.MultiExpressOnePack == null) // 自动判断的话，就看快递单号数量是不是大于1
        {
            if (expressTrackingNoList.Count > 1) {
                multiExpressOnePack = true;
            } else {
                multiExpressOnePack = false;
            }
        } else {
            multiExpressOnePack = param.MultiExpressOnePack.Value;
        }

        /* 去掉这个检查，现在自动判断是否合箱的，不一定每个打包记录都是一致的
        {
            IList<LogisticsOrderPackInfo> orderPackList = session.Query<LogisticsOrderPackInfo>().Where(e => e.Order.Id == orderId).ToList();
            IList<bool> multiExpressOnePackValueList = orderPackList
                .Where(e => e.MultiExpressOnePack != null)
                .Select(e => e.MultiExpressOnePack.Value)
                .Distinct()
                .ToList();
            if (multiExpressOnePackValueList.Count > 1)
            {
                throw new ValidationException("已打包包裹是否合箱设定不一致！");
            }

            if (multiExpressOnePackValueList.Count > 0 && multiExpressOnePack != multiExpressOnePackValueList[0])
            {
                if (multiExpressOnePack == false)
                {
                    throw new ValidationException("之前已打包包裹为【合箱】，当前为【不合箱】，两者不一致！");
                }
                else
                {
                    throw new ValidationException("之前已打包包裹为【不合箱】，当前为【合箱】，两者不一致！");
                }
            }
        }
        */

        // 获取物品类型快递单号列表
        IList<LogisticsOrderGoodsExpressInfo> goodsTypeGoodsExpressList = orderGoodsExpressList; // 默认取所有，一般空运时就是所有

        // 如果是空运和快递订单的，合并快递单号的物品类型，取用订单的物品类型，所以这里就不用区分了
        if (order.FirstModeType != LogisticsModeType.CnAuAir && order.FirstModeType != LogisticsModeType.CnAuExpress) // 非空运的时候过滤
        {
            goodsTypeGoodsExpressList = orderGoodsExpressList.Where(e => e.GoodsType == param.GoodsType).ToList();
        }

        IList<string> goodsTypeExpressTrackingNoList = goodsTypeGoodsExpressList
            .Select(e => e.ExpressTrackingNo)
            .ToList();

        int expressCountNotInGoodsType = param.ExpressTrackingNoList.Where(e => goodsTypeExpressTrackingNoList.Contains(e) == false).Count();
        if (expressCountNotInGoodsType != 0) {
            throw new ValidationException("快递单号不全属于当前订单当前物品类型！");
        }

        // 检查是不是有已经打包状态的
        IList<LogisticsOrderGoodsExpressInfo> orderExpressToPackList = orderGoodsExpressList
            .Where(e => expressTrackingNoList.Contains(e.ExpressTrackingNo))
            .ToList();

        IList<string> packedExpressTrackingNoList = orderExpressToPackList
            .Where(e => e.Status == LogisticsOrderGoodsExpressStatus.Packed)
            .Select(e => e.ExpressTrackingNo)
            .ToList();

        if (packedExpressTrackingNoList.Count != 0) {
            throw new ValidationException($"快递单号{string.Join(",", packedExpressTrackingNoList)}，已经是打包状态，不能继续打包！");
        }


        // 体积重快递单号HOLD检查
        VolumeWeightHoldExpressInfo volumeWeightHoldExpress = session.Query<VolumeWeightHoldExpressInfo>()
            .Where(e => e.Order.Id == orderId && e.Status == VolumeWeightHoldExpressStatus.Hold)
            .Take(1)
            .SingleOrDefault();

        if (volumeWeightHoldExpress != null) {
            throw new ValidationException("订单有快递单号被体积重量HOLD，不能打包！");
        }

        if (logisticsOrderGoodsExpressCustomerHoldService.CheckOrderHold(orderId)) {
            throw new ValidationException("订单有快递单号【面单客户ID】与实际【订单客户ID】不匹配，不能打包！");
        }

        /*
        int expressCountNotChecked = goodsTypeExpressTrackingNoList.Where(e => param.ExpressTrackingNoList.Contains(e) == false).Count();
        if (expressCountNotChecked != 0)
        {
            throw new ValidationException("当前物品类型快递单号未全部扫描！");
        }
        */

        LogisticsOrderPackInfo orderPack = new LogisticsOrderPackInfo();

        bool isCreate = true;
        if (param.PackId.HasValue && param.PackId.Value != 0) {
            isCreate = false;
            orderPack = session.Get<LogisticsOrderPackInfo>(param.PackId.Value);
            if (orderPack == null) {
                throw new ValidationException("指定的打包数据不存在！");
            }
        }

        orderPack.Order = order;
        orderPack.GoodsType = param.GoodsType;
        orderPack.Length = param.Length ?? 0;
        orderPack.Width = param.Width ?? 0;
        orderPack.Height = param.Height ?? 0;
        orderPack.Weight = param.Weight ?? 0;
        orderPack.PackageCount = param.PackageCount ?? 1;
        orderPack.MultiExpressOnePack = multiExpressOnePack; // 这个是前面判断好的结果

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

        foreach (LogisticsOrderGoodsExpressInfo express in orderExpressToPackList) {
            LogisticsOrderPackExpressInfo orderPackExpress = session
                .Query<LogisticsOrderPackExpressInfo>()
                .Where(e => e.Order.Id == orderId && e.OrderPack.Id == orderPack.Id && e.OrderExpress.Id == express.Id)
                .Take(1)
                .SingleOrDefault();

            if (orderPackExpress == null) {
                orderPackExpress = new LogisticsOrderPackExpressInfo();

                orderPackExpress.Order = session.Load<LogisticsOrderInfo>(orderId);
                orderPackExpress.GoodsType = express.GoodsType;
                orderPackExpress.OrderPack = orderPack;
                orderPackExpress.OrderExpress = express;
                orderPackExpress.ExpressNo = express.ExpressTrackingNo;

                session.Save(orderPackExpress);
            }

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
            orderPackBox.Customer = order.Customer;
            orderPackBox.Order = order;
            orderPackBox.OrderPack = orderPack;
            orderPackBox.OrderPackBoxNum = logisticsOrderPackBoxService.GetNextBoxNum(orderId);
            orderPackBox.ShipContainerCustomer = null;
            orderPackBox.ShipContainerCustomerEntry = null;
            orderPackBox.ShipContainerInner = null;
            orderPackBox.ShipContainerInnerEntry = null;
            orderPackBox.Status = LogisticsOrderPackBoxStatus.Pack;
            // 只要是人工打包的，都生成这个别名。放在 原来自动打包时可能用到的快递单号别名里面就行了，也不加别的字段了
            orderPackBox.AliasTrackingNo = logisticsOrderPackBoxService.GetBoxCodeOrderIdAlias(order.Id, orderPackBox.OrderPackBoxNum);

            session.Save(orderPackBox);
            session.Flush();

            orderPackBoxList.Add(orderPackBox);
        }

        // 箱号历史
        foreach (LogisticsOrderPackBoxInfo packBox in orderPackBoxList) {
            logisticsOrderPackBoxHistoryService.AddHistory(packBox, LogisticsOrderPackBoxHistoryType.Pack, loginUserId);
        }

        session.Flush();

        // 添加箱号至订单打包分拆
        bool packSplit = false;
        if (splitSetting.Status == LogisticsOrderPackSplitSettingStatus.Enabled) {
            packSplit = logisticsOrderPackSplitService.SplitAddPackBox(orderPackBoxList, orderExpressWeightTotal, orderPackSplitList, splitSetting);
        }

        // 是否是继续打包这些快递单号，如果不是继续打包这些快递单号的，那么就释放这些快递单号占用的仓位
        if (param.ContinueThoseExpress == false) {
            // 更新快递单号状态为已打包
            foreach (LogisticsOrderGoodsExpressInfo express in orderExpressToPackList) {
                express.Status = LogisticsOrderGoodsExpressStatus.Packed;
                session.Update(express);
            }

            session.Flush();

            // 更新快递单号下面物品状态为已打包
            IList<int> expressIdList = orderExpressToPackList.Select(e => e.Id).ToList();
            IList<LogisticsOrderGoodsInfo> orderGoodsList = session.Query<LogisticsOrderGoodsInfo>()
                .Fetch(e => e.Express)
                .Where(e => expressIdList.Contains(e.Express.Id))
                .ToList();

            foreach (LogisticsOrderGoodsInfo orderGoods in orderGoodsList) {
                // 同时把占用的仓位释放
                // 改成不清除仓位，让界面上还继续显示 orderGoods.StorageLoc = null; // 这个时候订单的起始仓位会可能无效
                orderGoods.Status = LogisticsOrderGoodsStatus.Packed;
                session.Update(orderGoods);
            }

            session.Flush();
        }

        // 如果当前状态是已打包 标记 物品，快递单号为 等待打包状态，同时订单也是
        // 这里调用这个有问题吧 2020-09-22
        // UpdateRepackStatus(expressTrackingNoList);


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

        // 空运订单判断 打包后重量是不是跟打包前重量比要大，大了就警告
        bool hasAirOrderWeightUnusual = false;
        if (order.Cate == LogisticsOrderCate.Air) {
            IList<LogisticsOrderPackInfo> orderPackList = session.Query<LogisticsOrderPackInfo>()
                .Where(e => e.Order.Id == orderId)
                .ToList();
            decimal orderPackWeightTotal = 0m;
            if (orderPackList.Count > 0) {
                orderPackWeightTotal = orderPackList.Sum(e => e.Weight * e.PackageCount);
            }

            if (orderPackWeightTotal > orderExpressWeightTotal) {
                hasAirOrderWeightUnusual = true;

                if (warningMessageBuilder.Length > 0) {
                    warningMessageBuilder.Append("<br /><br />");
                }

                warningMessageBuilder.Append("空运订单打包后总重量大于打包前总重量，请核查！");
            }
        }

        // 快递单号日志
        foreach (LogisticsOrderGoodsExpressInfo express in orderExpressToPackList) {
            expressLogService.UserExpressLog(express.Id, ExpressLogType.Pack, loginUserId);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "当前打包数据保存成功！" },
            { "orderPackId", orderPack.Id },
            { "hasPackWarning", hasDuplicatePackData || hasLengthUnusual || hasWeightUnusual || hasAirOrderWeightUnusual },
            { "packWarningMessage", warningMessageBuilder.ToString() },
        });
    }

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/remove/pack")]
    public IActionResult RemovePack(LogisticsOrderGoodsPackRemovePackParam param) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        IList<string> expressTrackingNoList = param.ExpressTrackingNoList;
        if (expressTrackingNoList == null || expressTrackingNoList.Count == 0) {
            throw new ValidationException("请扫描或输入快递单号，再移除打包信息！");
        }

        int orderId = param.OrderId;
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException("指定的订单不存在！");
        }

        LogisticsGoodsType goodsType = param.GoodsType;

        // 如果是空运和快递订单的，合并快递单号的物品类型，取用订单的物品类型
        if (order.FirstModeType == LogisticsModeType.CnAuAir || order.FirstModeType == LogisticsModeType.CnAuExpress) {
            goodsType = order.GoodsType; // 这个语句感觉不写也可以的，界面上传递的时候已经是订单的物品类型了
        }

        int orderPackId = param.PackId;

        // 获取订单快递单号列表
        IList<LogisticsOrderGoodsExpressInfo> goodsTypeExpressList = session
            .Query<LogisticsOrderGoodsExpressInfo>()
            .Where(e => e.Order.Id == orderId && e.GoodsType == goodsType)
            .ToList();

        IList<string> goodsTypeExpressTrackingNoList = goodsTypeExpressList
            .Select(e => e.ExpressTrackingNo)
            .ToList();

        int expressCountNotInGoodsType = param.ExpressTrackingNoList.Where(e => goodsTypeExpressTrackingNoList.Contains(e) == false).Count();
        if (expressCountNotInGoodsType != 0) {
            throw new ValidationException("快递单号不全属于当前物品类型！");
        }

        /*
        int expressCountNotChecked = goodsTypeExpressTrackingNoList.Where(e => param.ExpressTrackingNoList.Contains(e) == false).Count();
        if (expressCountNotChecked != 0)
        {
            throw new ValidationException("当前物品类型快递单号未全部扫描！");
        }
        */

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

        // 删除打包箱信息
        IList<LogisticsOrderPackBoxInfo> orderPackBoxList = session.Query<LogisticsOrderPackBoxInfo>()
            .Where(e => e.Order.Id == orderId && e.OrderPack.Id == orderPackId)
            .ToList();

        foreach (LogisticsOrderPackBoxInfo orderPackBox in orderPackBoxList) {
            session.Delete(orderPackBox);
        }

        session.Flush();

        session.Delete(orderPack);
        session.Flush();

        // 如果当前状态是已打包 标记 物品，快递单号为 等待打包状态，同时订单也是
        UpdateRepackStatus(orderId, expressTrackingNoList);

        // 快递单号日志
        int loginUserId = loginUserService.GetLoginUserId();
        foreach (LogisticsOrderGoodsExpressInfo orderExpress in goodsTypeExpressList) {
            expressLogService.UserExpressLog(orderExpress.Id, ExpressLogType.RemovePack, loginUserId);
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

        IList<LogisticsOrderGoodsExpressInfo> expressList = session.Query<LogisticsOrderGoodsExpressInfo>()
            .Where(e => e.Order.Id == orderId && expressTrackingNoList.Contains(e.ExpressTrackingNo))
            .ToList();

        foreach (var express in expressList) {
            if (express.Status == LogisticsOrderGoodsExpressStatus.Packed) {
                express.Status = LogisticsOrderGoodsExpressStatus.WaitPack;
                session.Update(express);
            }
        }

        session.Flush();

        IList<int> expressIdList = expressList.Select(e => e.Id).ToList();
        IList<LogisticsOrderGoodsInfo> orderGoodsList = session.Query<LogisticsOrderGoodsInfo>()
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();

        foreach (var orderGoods in orderGoodsList) {
            if (orderGoods.Status == LogisticsOrderGoodsStatus.Packed) {
                orderGoods.Status = LogisticsOrderGoodsStatus.WaitPack;
                session.Update(orderGoods);
            }
        }

        session.Flush();

        // 回写申领件状态，这个操作也跟3.0的操作可以兼容
        session.Query<LogisticsClaimGoodsInfo>()
            .Where(e => expressTrackingNoList.Contains(e.TrackingNo))
            .UpdateBuilder()
            .Set(e => e.Picked, true)
            .Set(e => e.WarehouseProcessed, true)
            .Set(e => e.Status, LogisticsClaimGoodsStatus.WaitPack)
            .Set(e => e.UpdateTime, DateTime.Now)
            .Update();

        /*
         感觉这里的逻辑有些不对 20200924
         
        int packedCount = expressList.Where(e => e.Status == LogisticsOrderGoodsExpressStatus.Packed).Count();
        if (logisticsOrder.OrderStatus == LogisticsOrderStatus.Packed
            && packedCount == expressList.Count) // 都是已打包状态，一般状态都是一致的
        {
            logisticsOrder.OrderStatus = LogisticsOrderStatus.WaitPack;
            session.Update(logisticsOrder);
            session.Flush();

            foreach (var express in expressList)
            {
                express.Status = LogisticsOrderGoodsExpressStatus.WaitPack;
                session.Update(express);
            }

            session.Flush();

            IList<int> expressIdList = expressList.Select(e => e.Id).ToList();
            IList<LogisticsOrderGoodsInfo> orderGoodsList = session.Query<LogisticsOrderGoodsInfo>()
                .Where(e => expressIdList.Contains(e.Express.Id))
                .ToList();

            foreach (var orderGoods in orderGoodsList)
            {
                orderGoods.Status = LogisticsOrderGoodsStatus.WaitPack;
                session.Update(orderGoods);
            }

            session.Flush();
        }
        */
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

        if ((goodsExpress.Order.Warehouse?.Id ?? 0) != warehouseId) {
            WarehouseInfo warehouse = session.Get<WarehouseInfo>(warehouseId);
            if (warehouse == null) {
                throw new ValidationException($"指定的收货仓库 #{warehouseId} 不存在！");
            }

            throw new ValidationException($"本快递对应的订单的收货仓 {goodsExpress.Order.Warehouse.Name}，不是当前仓库 {warehouse.Name}！");
        }

        int goodsExpressOrderId = goodsExpress.Order?.Id ?? 0;

        if (loginUserService.IsAdmin == false) {
            LogisticsOrderPackScheduleInfo orderPackSchedule = session.Query<LogisticsOrderPackScheduleInfo>()
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

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/complete/goods/type/pack")]
    public IActionResult CompleteGoodsTypePack(LogisticsOrderGoodsPackCompleteGoodsTypePackParam param) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        int orderId = param.OrderId;
        LogisticsGoodsType goodsType = param.GoodsType;

        LogisticsOrderInfo logisticsOrder = session.Get<LogisticsOrderInfo>(orderId);
        if (logisticsOrder == null) {
            throw new ValidationException("指定的订单不存在！");
        }

        /* 不检查这个了，后面有对打包时记录的快递单号检查
        IList<string> expressTrackingNoList = param.ExpressTrackingNoList;
        if (expressTrackingNoList == null || expressTrackingNoList.Count == 0)
        {
            throw new ValidationException("请扫描或输入快递单号，再完成打包！");
        }
        */

        /*
        // 获取订单快递单号列表
        IList<string> goodsTypeExpressTrackingNoList = session
            .Query<LogisticsOrderGoodsExpressInfo>()
            .Where(e => e.Order.Id == orderId && e.GoodsType == goodsType)
            .Select(e => e.ExpressTrackingNo)
            .ToList();

        int expressCountNotInGoodsType = param.ExpressTrackingNoList.Where(e => goodsTypeExpressTrackingNoList.Contains(e) == false).Count();
        if (expressCountNotInGoodsType != 0)
        {
            throw new ValidationException("快递单号不全属于当前物品类型！");
        }
        */

        /* 这个为什么不用了
        int expressCountNotChecked = goodsTypeExpressTrackingNoList.Where(e => param.ExpressTrackingNoList.Contains(e) == false).Count();
        if (expressCountNotChecked != 0)
        {
            throw new ValidationException("当前物品类型快递单号未全部扫描！");
        }
        */

        // 检查当前物品类型中的快递单号是不是都已经打包了
        var goodsTypeExpressNoQuery = session
            .Query<LogisticsOrderGoodsExpressInfo>()
            .Where(e => e.Order.Id == orderId);

        // 空运和快递订单打包时，各个物品类型的合并打包
        if (logisticsOrder.FirstModeType != LogisticsModeType.CnAuAir && logisticsOrder.FirstModeType != LogisticsModeType.CnAuExpress) {
            goodsTypeExpressNoQuery = goodsTypeExpressNoQuery.Where(e => e.GoodsType == goodsType);
        }

        IList<string> goodsTypeExpressNoList = goodsTypeExpressNoQuery
            .Select(e => e.ExpressTrackingNo)
            .Distinct()
            .ToList();

        var packedGoodsTypeExpressNoQuery = session.Query<LogisticsOrderPackExpressInfo>()
            .Where(e => e.Order.Id == orderId);

        // 空运和快递订单打包时，各个物品类型的合并打包
        if (logisticsOrder.FirstModeType != LogisticsModeType.CnAuAir && logisticsOrder.FirstModeType != LogisticsModeType.CnAuExpress) {
            packedGoodsTypeExpressNoQuery = packedGoodsTypeExpressNoQuery.Where(e => e.GoodsType == goodsType);
        }

        IList<string> packedGoodsTypeExpressNoList = packedGoodsTypeExpressNoQuery
            .Select(e => e.ExpressNo)
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
            .Query<LogisticsOrderGoodsExpressInfo>()
            .Where(e => e.Order.Id == orderId);

        // 空运和快递订单打包时，各个物品类型的合并打包
        if (logisticsOrder.FirstModeType != LogisticsModeType.CnAuAir && logisticsOrder.FirstModeType != LogisticsModeType.CnAuExpress) {
            orderGoodsExpressQuery = orderGoodsExpressQuery.Where(e => e.GoodsType == goodsType);
        }

        IList<LogisticsOrderGoodsExpressInfo> orderGoodsExpressList = orderGoodsExpressQuery.ToList();

        foreach (LogisticsOrderGoodsExpressInfo orderGoodsExpress in orderGoodsExpressList) {
            orderGoodsExpress.Status = LogisticsOrderGoodsExpressStatus.Packed;
            session.Update(orderGoodsExpress);
        }

        session.Flush();

        // 更新物品状态 这里是按物品类型来取物品列表的
        var expressGoodsTypeOrderGoodsQuery = session.Query<LogisticsOrderGoodsInfo>()
            .Fetch(e => e.Express)
            .Where(e => e.Express.Order.Id == orderId);

        // 空运和快递订单打包时，各个物品类型的合并打包
        if (logisticsOrder.FirstModeType != LogisticsModeType.CnAuAir && logisticsOrder.FirstModeType != LogisticsModeType.CnAuExpress) {
            expressGoodsTypeOrderGoodsQuery = expressGoodsTypeOrderGoodsQuery.Where(e => e.Express.GoodsType == goodsType); // 物品类型要按快递单号来，不是按下面的物品来
        }

        IList<LogisticsOrderGoodsInfo> expressGoodsTypeOrderGoodsList = expressGoodsTypeOrderGoodsQuery.ToList();

        // 更新物品状态 这里是按快递单号来取物品列表的, 也许这个方式更合理, 但上面的方式已经运行了多时没有出现过错误, 就先用上面的方式好了

        /*
        IList<int> expressIdList = orderGoodsExpressList.Select(e => e.Id).ToList();

        IList<LogisticsOrderGoodsInfo> expressGoodsTypeOrderGoodsList = session.Query<LogisticsOrderGoodsInfo>()
            .Fetch(e => e.Express)
            .Where(e => e.Express.Order.Id == orderId && expressIdList.Contains(e.Express.Id))
            .ToList();
        */

        // 更新物品状态
        foreach (LogisticsOrderGoodsInfo orderGoods in expressGoodsTypeOrderGoodsList) {
            // 同时把占用的仓位释放
            // 改成不清除仓位，让界面上还继续显示 orderGoods.StorageLoc = null;
            orderGoods.Status = LogisticsOrderGoodsStatus.Packed;
            session.Update(orderGoods);
        }

        session.Flush();

        // 更新订单状态
        int notPackedExpressCount = session.Query<LogisticsOrderGoodsExpressInfo>()
            .Where(e => e.Order.Id == orderId && e.Status != LogisticsOrderGoodsExpressStatus.Packed)
            .Count();

        // 是否订单完成打包
        bool orderPackComplete = notPackedExpressCount == 0;

        bool tlaShip = false;
        bool transDirectShip = false;
        int tlaPrintCount = 0;
        if (orderPackComplete) {
            Tuple<DeliverSupplierType, int> result = logisticsOrderService.CompleteOrderPackManual(orderId);

            tlaShip = result.Item1 == DeliverSupplierType.Tla || result.Item1 == DeliverSupplierType.TransDirect; // TransDirect 匹配的时候也出tla的自提标签
            transDirectShip = result.Item1 == DeliverSupplierType.TransDirect;
            tlaPrintCount = result.Item2;
        }

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "orderPackComplete", orderPackComplete },
            { "tlaShip", tlaShip },
            { "transDirectShip", transDirectShip },
            { "tlaPrintCount", tlaPrintCount },
            { "message", "完成订单当前物品类型打包成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/complete/order/pack")]
    public IActionResult CompleteOrderPack(LogisticsOrderGoodsPackCompleteOrderPackParam param) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        int orderId = param.OrderId;

        Tuple<DeliverSupplierType, int> result = logisticsOrderService.CompleteOrderPackManual(orderId);

        bool tlaShip = result.Item1 == DeliverSupplierType.Tla || result.Item1 == DeliverSupplierType.TransDirect; // TransDirect 匹配的时候也出tla的自提标签
        bool transDirectShip = result.Item1 == DeliverSupplierType.TransDirect;
        int tlaPrintCount = result.Item2;

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "tlaShip", tlaShip },
            { "transDirectShip", transDirectShip },
            { "tlaPrintCount", tlaPrintCount },
            { "message", "完成订单打包成功！" }
        });
    }

    #endregion

    #region 标签打印信息

    [HttpPost]
    [Route("/admin/logistics/order/goods/pack/get/print/pack/data/10/15")]
    [NoLoginRequired]
    public IActionResult GetPrintPackDataW10H15(int orderPackId) {
        LogisticsOrderPackConfigInfo logisticsOrderPackConfig = session
            .Query<LogisticsOrderPackConfigInfo>()
            .Take(1)
            .SingleOrDefault();

        if (logisticsOrderPackConfig == null) {
            throw new Exception("打包配置还没配置，请先配置订单打包！");
        }

        int printCount = logisticsOrderPackConfig.Label1015PrintCount;

        LogisticsOrderPackInfo orderPack = session.Get<LogisticsOrderPackInfo>(orderPackId);
        if (orderPack == null) {
            throw new ValidationException("指定的订单打包信息不存在！");
        }

        IList<LogisticsOrderPackBoxInfo> orderPackBoxList = session
            .Query<LogisticsOrderPackBoxInfo>()
            .Where(e => e.OrderPack.Id == orderPackId)
            .ToList();

        // string customerCode = orderPack.Order?.CustomerCode;
        string customerCode = (orderPack.Order?.Customer?.Id ?? 0).ToString();
        string orderCode = orderPack.Order?.Code;

        IList<string> customerCodeList = new List<string>();
        IList<string> packSplitCodeList = new List<string>();
        IList<string> orderIdList = new List<string>();
        IList<string> orderCodeList = new List<string>();
        IList<string> orderPackBoxCodeList = new List<string>();
        IList<string> orderPackBoxShortCodeList = new List<string>();
        IList<int> orderPackBoxNumList = new List<int>();
        foreach (LogisticsOrderPackBoxInfo orderPackBox in orderPackBoxList) {
            string splitSubCode = orderPackBox.OrderPackSplit?.SubCode ?? string.Empty;
            if (string.IsNullOrWhiteSpace(splitSubCode)) {
                if (orderPackBox.Order.PackSplitStatus == LogisticsOrderPackSplitStatus.NotSplitWithSuffix) {
                    splitSubCode = LogisticsOrderPackSplitService.FirstSubCode;
                }
            }

            // customerCodeList.Add(customerCode + splitSubCode);
            customerCodeList.Add(customerCode);
            packSplitCodeList.Add(splitSubCode);
            orderCodeList.Add(orderCode + splitSubCode);

            if (orderPackBox.OrderPackSplit != null) {
                orderIdList.Add(orderPackBox.OrderPackSplit.Id.ToString());
            } else {
                orderIdList.Add(orderPackBox.Order.Id.ToString());
            }

            orderPackBoxCodeList.Add(logisticsOrderPackBoxService.GetBoxCode(orderPackBox));
            orderPackBoxShortCodeList.Add(logisticsOrderPackBoxService.GetBoxShortCode(orderPackBox));
            orderPackBoxNumList.Add(orderPackBox.OrderPackBoxNum);
        }

        int orderId = orderPack.Order?.Id ?? 0;
        // string firstLogisticsTypeName = orderPack.Order?.FirstModeType?.Name;
        string firstLogisticsTypeName = orderPack.Order?.FirstMode?.Name;
        string targetWarehouseName = orderPack.Order?.TargetWarehouse?.Name;
        string secondLogisticsTypeName = orderPack.Order?.SecondLogisticsModeType?.Name;
        if (orderPack.Order?.SecondLogisticsModeType == LogisticsModeType.TerminalPickup) {
            secondLogisticsTypeName = orderPack.Order?.SecondLogisticsMode?.PickupStation?.Name;
        }

        // string weight = orderPack.Weight.ToString("#0.000") + "Kg";
        // string volumeWeight = (orderPack.Width * orderPack.Length * orderPack.Height / 5000m).ToString("#0.000") + "Kg";
        // string volume = (orderPack.Width * orderPack.Length * orderPack.Height / 1000000m).ToString("#0.000000") + "立方";
        string weight = orderPack.Weight.ToString("#0.0") + "Kg";
        string volumeWeight = (orderPack.Width * orderPack.Length * orderPack.Height / 5000m).ToString("#0.0") + "Kg";
        string volume = (orderPack.Width * orderPack.Length * orderPack.Height / 1000000m).ToString("#0.00") + "立方";
        string lwh = $"{orderPack.Length:#} x {orderPack.Width:#} x {orderPack.Height:#}CM";
        // string userName = session.Get<UserInfo>(loginUserService.GetLoginUserId())?.UserName;
        string userName = orderPack.PackUser?.UserName ?? "";
        string packTime = orderPack.PackTime.ToString("MM-dd HH:mm");
        string expressGoodsTypeName = orderPack.GoodsType?.Name;
        string orderGoodsTypeName = orderPack.Order?.GoodsType?.Name;

        // 放在这个Action操作的最后， 保存别的不出错时，才记录
        logisticsOrderPackLabelTypeService.AddOrderPackLabelType(orderPack, LogisticsOrderPackPrintType.SizeW10H15);

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "customerCode", customerCode }, // 这是原来的主订单客户ID
            { "orderCode", orderCode }, // 这是原来的主订单编号
            { "orderId", orderId },
            { "firstLogisticsTypeName", firstLogisticsTypeName },
            { "targetWarehouseName", targetWarehouseName },
            { "secondLogisticsTypeName", secondLogisticsTypeName },
            { "weight", weight },
            { "volumeWeight", volumeWeight },
            { "volume", volume },
            { "lwh", lwh },
            { "customerCodeList", customerCodeList },
            { "packSplitCodeList", packSplitCodeList },
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
        LogisticsOrderPackConfigInfo logisticsOrderPackConfig = session
            .Query<LogisticsOrderPackConfigInfo>()
            .Take(1)
            .SingleOrDefault();

        if (logisticsOrderPackConfig == null) {
            throw new Exception("打包配置还没配置，请先配置订单打包！");
        }

        int printCount = logisticsOrderPackConfig.Label1005PrintCount;

        LogisticsOrderPackInfo orderPack = session.Get<LogisticsOrderPackInfo>(orderPackId);
        if (orderPack == null) {
            throw new ValidationException("指定的订单打包信息不存在！");
        }

        IList<LogisticsOrderPackBoxInfo> orderPackBoxList = session
            .Query<LogisticsOrderPackBoxInfo>()
            .Where(e => e.OrderPack.Id == orderPackId)
            .ToList();

        // string customerCode = orderPack.Order?.CustomerCode;
        string customerCode = (orderPack.Order?.Customer?.Id ?? 0).ToString();
        string orderCode = orderPack.Order?.Code;

        IList<string> customerCodeList = new List<string>();
        IList<string> orderIdList = new List<string>();
        IList<string> orderCodeList = new List<string>();
        IList<string> orderPackBoxCodeList = new List<string>();
        IList<string> orderPackBoxShortCodeList = new List<string>();
        IList<int> orderPackBoxNumList = new List<int>();
        foreach (LogisticsOrderPackBoxInfo orderPackBox in orderPackBoxList) {
            string splitSubCode = orderPackBox.OrderPackSplit?.SubCode ?? string.Empty;
            if (string.IsNullOrWhiteSpace(splitSubCode)) {
                if (orderPackBox.Order.PackSplitStatus == LogisticsOrderPackSplitStatus.NotSplitWithSuffix) {
                    splitSubCode = LogisticsOrderPackSplitService.FirstSubCode;
                }
            }

            customerCodeList.Add(customerCode + splitSubCode);
            orderCodeList.Add(orderCode + splitSubCode);

            if (orderPackBox.OrderPackSplit != null) {
                orderIdList.Add(orderPackBox.OrderPackSplit.Id.ToString());
            } else {
                orderIdList.Add(orderPackBox.Order.Id.ToString());
            }

            orderPackBoxCodeList.Add(logisticsOrderPackBoxService.GetBoxCode(orderPackBox));
            orderPackBoxShortCodeList.Add(logisticsOrderPackBoxService.GetBoxShortCode(orderPackBox));
            orderPackBoxNumList.Add(orderPackBox.OrderPackBoxNum);
        }

        // string firstLogisticsTypeName = orderPack.Order?.FirstModeType?.Name;
        string firstLogisticsTypeName = orderPack.Order?.FirstMode?.Name;
        string targetWarehouseName = orderPack.Order?.TargetWarehouse?.Name;
        string secondLogisticsTypeName = orderPack.Order?.SecondLogisticsModeType?.Name;
        if (orderPack.Order?.SecondLogisticsModeType == LogisticsModeType.TerminalPickup) {
            secondLogisticsTypeName = orderPack.Order?.SecondLogisticsMode?.PickupStation?.Name;
        }

        // string weight = orderPack.Weight.ToString("#0.000") + "Kg";
        // string volumeWeight = (orderPack.Width * orderPack.Length * orderPack.Height / 5000m).ToString("#0.000") + "Kg";
        // string volume = (orderPack.Width * orderPack.Length * orderPack.Height / 1000000m).ToString("#0.000000") + "立方";
        string weight = orderPack.Weight.ToString("#0.0") + "Kg";
        string volumeWeight = (orderPack.Width * orderPack.Length * orderPack.Height / 5000m).ToString("#0.0") + "Kg";
        string volume = (orderPack.Width * orderPack.Length * orderPack.Height / 1000000m).ToString("#0.00") + "方";
        // string userName = session.Get<UserInfo>(loginUserService.GetLoginUserId())?.UserName;
        string userName = orderPack.PackUser?.UserName ?? "";
        string packTime = orderPack.PackTime.ToString("MM-dd HH:mm");
        string expressGoodsTypeName = orderPack.GoodsType?.Code;
        string orderGoodsTypeName = orderPack.Order?.GoodsType?.Code;

        // 放在这个Action操作的最后， 保存别的不出错时，才记录
        logisticsOrderPackLabelTypeService.AddOrderPackLabelType(orderPack, LogisticsOrderPackPrintType.SizeW10H5);

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "customerCode", customerCode },
            { "orderCode", orderCode },
            { "firstLogistics", firstLogisticsTypeName?.Trim()?.FirstOrDefault() },
            { "targetWarehouseName", targetWarehouseName?.Trim()?.FirstOrDefault() },
            { "secondLogistics", secondLogisticsTypeName },
            { "secondLogisticsShortName", orderPack.Order?.SecondLogisticsModeType?.GetCnAuSecondTypeShortName() },
            { "terminalShortName", orderPack.Order?.SecondLogisticsMode?.PickupStation?.Name?.FirstOrDefault() },
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
        LogisticsOrderPackConfigInfo logisticsOrderPackConfig = session
            .Query<LogisticsOrderPackConfigInfo>()
            .Take(1)
            .SingleOrDefault();

        if (logisticsOrderPackConfig == null) {
            throw new Exception("打包配置还没配置，请先配置订单打包！");
        }

        int printCount = logisticsOrderPackConfig.Label0505PrintCount;

        LogisticsOrderPackInfo orderPack = session.Get<LogisticsOrderPackInfo>(orderPackId);
        if (orderPack == null) {
            throw new ValidationException("指定的订单打包信息不存在！");
        }

        IList<LogisticsOrderPackBoxInfo> orderPackBoxList = session
            .Query<LogisticsOrderPackBoxInfo>()
            .Where(e => e.OrderPack.Id == orderPackId)
            .ToList();

        // string customerCode = orderPack.Order?.CustomerCode;
        string customerCode = (orderPack.Order?.Customer?.Id ?? 0).ToString();
        string orderCode = orderPack.Order?.Code;

        IList<string> customerCodeList = new List<string>();
        IList<string> orderIdList = new List<string>();
        IList<string> orderCodeList = new List<string>();
        IList<string> orderPackBoxCodeList = new List<string>();
        IList<string> orderPackBoxShortCodeList = new List<string>();
        IList<int> orderPackBoxNumList = new List<int>();
        foreach (LogisticsOrderPackBoxInfo orderPackBox in orderPackBoxList) {
            string splitSubCode = orderPackBox.OrderPackSplit?.SubCode ?? string.Empty;
            if (string.IsNullOrWhiteSpace(splitSubCode)) {
                if (orderPackBox.Order.PackSplitStatus == LogisticsOrderPackSplitStatus.NotSplitWithSuffix) {
                    splitSubCode = LogisticsOrderPackSplitService.FirstSubCode;
                }
            }

            customerCodeList.Add(customerCode + splitSubCode);
            orderCodeList.Add(orderCode + splitSubCode);

            if (orderPackBox.OrderPackSplit != null) {
                orderIdList.Add(orderPackBox.OrderPackSplit.Id.ToString());
            } else {
                orderIdList.Add(orderPackBox.Order.Id.ToString());
            }

            orderPackBoxCodeList.Add(logisticsOrderPackBoxService.GetBoxCode(orderPackBox));
            orderPackBoxShortCodeList.Add(logisticsOrderPackBoxService.GetBoxShortCode(orderPackBox));
            orderPackBoxNumList.Add(orderPackBox.OrderPackBoxNum);
        }

        // string firstLogisticsTypeName = orderPack.Order?.FirstModeType?.Name;
        string firstLogisticsTypeName = orderPack.Order?.FirstMode?.Name;
        string targetWarehouseName = orderPack.Order?.TargetWarehouse?.Name;
        string secondLogisticsTypeName = orderPack.Order?.SecondLogisticsModeType?.Name;
        if (orderPack.Order?.SecondLogisticsModeType == LogisticsModeType.TerminalPickup) {
            secondLogisticsTypeName = orderPack.Order?.SecondLogisticsMode?.PickupStation?.Name;
        }

        // string weight = orderPack.Weight.ToString("#0.000") + "Kg";
        // string volumeWeight = (orderPack.Width * orderPack.Length * orderPack.Height / 5000m).ToString("#0.000") + "Kg";
        // string volume = (orderPack.Width * orderPack.Length * orderPack.Height / 1000000m).ToString("#0.000000") + "立方";
        string weight = orderPack.Weight.ToString("#0.0") + "Kg";
        string volumeWeight = (orderPack.Width * orderPack.Length * orderPack.Height / 5000m).ToString("#0.0") + "Kg";
        string volume = (orderPack.Width * orderPack.Length * orderPack.Height / 1000000m).ToString("#0.00") + "方";
        // string userName = session.Get<UserInfo>(loginUserService.GetLoginUserId())?.UserName;
        string userName = orderPack.PackUser?.UserName ?? "";
        string packTime = orderPack.PackTime.ToString("MM-dd HH:mm");
        string expressGoodsTypeName = orderPack.GoodsType?.Code;
        string orderGoodsTypeName = orderPack.Order?.GoodsType?.Code;

        // 放在这个Action操作的最后， 保存别的不出错时，才记录
        logisticsOrderPackLabelTypeService.AddOrderPackLabelType(orderPack, LogisticsOrderPackPrintType.SizeW5H5);

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "customerCode", customerCode },
            { "orderCode", orderCode },
            { "firstLogistics", firstLogisticsTypeName?.Trim()?.FirstOrDefault() },
            { "targetWarehouseName", targetWarehouseName?.Trim()?.FirstOrDefault() },
            { "secondLogistics", secondLogisticsTypeName },
            { "secondLogisticsShortName", orderPack.Order?.SecondLogisticsModeType?.GetCnAuSecondTypeShortName() },
            { "terminalShortName", orderPack.Order?.SecondLogisticsMode?.PickupStation?.Name?.FirstOrDefault() },
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
        // string customerCode = order?.CustomerCode;
        string customerCode = (order?.Customer?.Id ?? 0).ToString();
        string orderCode = order?.Code;
        int orderId = order?.Id ?? 0;
        string firstLogisticsModeName = order?.FirstMode?.Name;
        string actualWeight = orderPack.Weight.ToString("#0.0") + "Kg";
        string volumeWeight = (orderPack.Width * orderPack.Length * orderPack.Height / 5000m).ToString("#0.0") + "Kg";
        string volume = (orderPack.Width * orderPack.Length * orderPack.Height / 1000000m).ToString("#0.00") + "立方";
        string userName = orderPack.PackUser?.UserName ?? "";
        string packTime = orderPack.PackTime.ToString("MM-dd HH:mm");
        string consignee = order?.Consignee + " " + order?.LastName;
        string mobile = order?.Mobile;
        string address = $"{order?.Address}, {order?.City} {order?.Province} {order?.Zipcode}";
        if (string.IsNullOrWhiteSpace(order?.UnitLoc) == false) {
            address = $"{order?.UnitLoc}, " + address;
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
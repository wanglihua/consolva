using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Domain.Storage;
using shunda.Service.Customer;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressListService {
    public static DateTime EXPRESS_CHECKIN_RECENT_AGO => DateTime.Today.AddDays(-90); // 90天之前的时间段，做为长期和近期的区分点  每次读取时都按当前日期计算

    private readonly ISession session;
    private readonly ExpressSizeWeightService expressSizeWeightService;
    private readonly CustomerCodeService customerCodeService;
    private readonly ExpressPhotoService expressPhotoService;

    public ExpressListService(ISession session,
        ExpressSizeWeightService expressSizeWeightService,
        CustomerCodeService customerCodeService,
        ExpressPhotoService expressPhotoService) {
        this.session = session;
        this.expressSizeWeightService = expressSizeWeightService;
        this.customerCodeService = customerCodeService;
        this.expressPhotoService = expressPhotoService;
    }

    /// <summary>
    /// 分页获取列表数据
    /// </summary>
    /// <param name="expressListMenuCate"></param>
    /// <param name="queryParam"></param>
    /// <param name="listPager"></param>
    /// <returns></returns>
    public IList<ExpressInfo> List(ExpressListMenuCate expressListMenuCate, ExpressListQueryParam queryParam, ListPager? listPager) {
        ICriteria criteria = GetListCriteria(expressListMenuCate, queryParam);

        if (listPager != null) {
            criteria = criteria.AddOrder(new Order(listPager.SortField, listPager.IsAscending))
                .AddOrder(Order.Desc(nameof(Entity.Id)))
                .SetFirstResult(listPager.PageIndex * listPager.PageSize)
                .SetMaxResults(listPager.PageSize);
        }

        IList<ExpressInfo> expressList = criteria.List<ExpressInfo>();

        IList<int> expressIdList = expressList.Select(e => e.Id).ToList();
        IList<string> trackingNoList = expressList.Select(e => e.TrackingNo).ToList();
        IList<int> orderIdList = expressList.Where(e => e.Order != null).Select(e => e.Order!.Id).Distinct().ToList();
        IList<int> firstModeIdList = expressList.Where(e => e.FirstMode != null).Select(e => e.FirstMode!.Id).Distinct().ToList();
        IList<int> warehouseIdList = expressList.Select(e => e.Warehouse.Id).Distinct().ToList();

        List<int> userIdList = expressList.Select(e => e.AddUser.Id).ToList();
        userIdList.AddRange(expressList.Where(e => e.OnShelfUser != null).Select(e => e.OnShelfUser!.Id));
        userIdList.AddRange(expressList.Where(e => e.OffShelfUser != null).Select(e => e.OffShelfUser!.Id));
        userIdList.AddRange(expressList.Where(e => e.PackingUser != null).Select(e => e.PackingUser!.Id));
        userIdList.AddRange(expressList.Where(e => e.DumpUser != null).Select(e => e.DumpUser!.Id));
        userIdList = userIdList.Distinct().ToList();


        List<int> storageLocIdList = expressList.Select(e => e.StorageLoc.Id).Distinct().ToList();
        storageLocIdList = storageLocIdList.Distinct().ToList();

        // 预加载
        IList<UserInfo> userList = session.Query<UserInfo>().Where(e => userIdList.Contains(e.Id)).ToList();

        // 预加载
        IList<StorageLocInfo> storageLocList = session.Query<StorageLocInfo>().Fetch(e => e.Spec).Fetch(e => e.Shelf).Where(e => storageLocIdList.Contains(e.Id)).ToList();

        // 预加载
        IList<WarehouseInfo> warehouseList = session.Query<WarehouseInfo>()
            .Where(e => warehouseIdList.Contains(e.Id))
            .ToList();

        // 预加载
        IList<LogisticsModeInfo> modeList = session.Query<LogisticsModeInfo>()
            .Where(e => firstModeIdList.Contains(e.Id))
            .ToList();

        // 订单字段比较多，这里只取 order status
        var orderStatusNameList = session.Query<LogisticsOrderInfo>()
            .Where(e => orderIdList.Contains(e.Id))
            .Select(e => new { OrderId = e.Id, OrderStatusName = e.OrderStatus.Name })
            .ToList();

        IList<ExpressSizeWeightInfo> expressSizeWeightList = session.Query<ExpressSizeWeightInfo>()
            // .Where(e => expressIdList.Contains(e.Express!.Id))
            .Where(e => trackingNoList.Contains(e.TrackingNo))
            .ToList();

        IList<ExpressExpectInfo> expressExpectList = session.Query<ExpressExpectInfo>()
            .Where(e => trackingNoList.Contains(e.TrackingNo))
            .ToList();

        IList<ExpressGoodsInfo> expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();

        IList<ExpressRemarkInfo> expressRemarkList = session.Query<ExpressRemarkInfo>()
            .Where(e => expressIdList.Contains(e.Express!.Id))
            .ToList();

        IList<ExpressPhotoInfo> expressPhotoList = expressPhotoService.GetExpressPhotoList(trackingNoList);

        foreach (ExpressInfo express in expressList) {
            int expressId = express.Id;
            string trackingNo = express.TrackingNo;

            express.CustomerCodeCache = customerCodeService.GetCustomerCode(express.Customer?.Id ?? 0);

            express.StorageLocNameCache = express.StorageLoc.GetFullName();

            var goodsListForExpress = expressGoodsList
                .Where(e => e.Express.Id == expressId)
                .ToList();
            express.GoodsNamesCache = string.Join(",", goodsListForExpress.Select(e => e.Goods.Name).ToArray());

            // express.ExpressSizeWeightListCache = expressSizeWeightList.Where(e => e.Express!.Id == expressId).ToList();
            express.ExpressSizeWeightListCache = expressSizeWeightList.Where(e => e.TrackingNo == trackingNo).ToList();
            express.ExpressRemarkListCache = expressRemarkList.Where(e => (e.Express?.Id ?? 0) == expressId).ToList();
            express.ExpressExpectCache = expressExpectList.Where(e => e.TrackingNo == trackingNo).Take(1).SingleOrDefault();

            express.ExpressPhotoListCache = expressPhotoList
                .Where(e => e.TrackingNo == express.TrackingNo)
                .OrderBy(e => e.Type.Id) // 必须要用 Type.Id, Type之间还不能比较
                .ToList();
        }

        return expressList;
    }

    /// <summary>
    /// 不分页获取列表数据
    /// </summary>
    /// <param name="expressListMenuCate"></param>
    /// <param name="queryParam"></param>
    /// <returns></returns>
    public IList<ExpressInfo> List(ExpressListMenuCate expressListMenuCate, ExpressListQueryParam queryParam) {
        return List(expressListMenuCate, queryParam, null);
    }

    /// <summary>
    /// 计数
    /// </summary>
    /// <param name="expressListMenuCate"></param>
    /// <param name="queryParam"></param>
    /// <returns></returns>
    public int Count(ExpressListMenuCate expressListMenuCate, ExpressListQueryParam queryParam) {
        int totalCount = GetListCriteria(expressListMenuCate, queryParam)
            .SetProjection(Projections.Count<ExpressInfo>(e => e.Id))
            .UniqueResult<int>();

        return totalCount;
    }

    public IList<ShelfInfo> GetShelfList(ExpressListMenuCate expressListMenuCate, int warehouseId) {
        if (expressListMenuCate != ExpressListMenuCate.ToOnShelfSchedule
            && expressListMenuCate != ExpressListMenuCate.ToOffShelfSchedule
            && expressListMenuCate != ExpressListMenuCate.NotPickedLongDumpSchedule
            && expressListMenuCate != ExpressListMenuCate.NotOrderLongDumpSchedule) {
            throw new ValidationException("该方法当前只支持排班获取货架列表！");
        }

        ICriteria criteria = session.CreateCriteria<ExpressInfo>();
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(ExpressInfo.StorageLoc), "sl");

        criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressInfo.Warehouse)}.{nameof(ExpressInfo.Warehouse.Id)}", warehouseId));

        // 待上架排班
        if (expressListMenuCate == ExpressListMenuCate.ToOnShelfSchedule) {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.ShelfStatus == ExpressShelfStatus.ToOnShelf));
        }

        // 待下架排班
        if (expressListMenuCate == ExpressListMenuCate.ToOffShelfSchedule) {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.WaitPack));
        }

        // 长期未认领待堆存排班
        if (expressListMenuCate == ExpressListMenuCate.NotPickedLongDumpSchedule) {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                e.AddTime < EXPRESS_CHECKIN_RECENT_AGO
                && e.Status == ExpressStatus.WaitPick
                && e.DumpStatus == ExpressDumpStatus.NotDump));
        }

        // 长期未出库待堆存排班
        if (expressListMenuCate == ExpressListMenuCate.NotOrderLongDumpSchedule) {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                e.AddTime < EXPRESS_CHECKIN_RECENT_AGO
                && e.Status == ExpressStatus.Checkin
                && e.DumpStatus == ExpressDumpStatus.NotDump));
        }

        // criteria = criteria.AddOrder(Order.Asc($"{nameof(ExpressInfo.StorageLoc)}.{nameof(ExpressInfo.StorageLoc.Shelf)}.{nameof(ExpressInfo.StorageLoc.Shelf.Name)}"));

        criteria = criteria.SetProjection(Projections.Distinct(Projections.Property($"sl.{nameof(ExpressInfo.StorageLoc.Shelf)}")));

        return criteria.List<ShelfInfo>().OrderBy(e => e.Name).ToList();
    }

    public IList<ShelfInfo> GetShelfList(ExpressListMenuCate expressListMenuCate, ExpressListQueryParam queryParam) {
        if (expressListMenuCate != ExpressListMenuCate.ToOnShelfSchedule
            && expressListMenuCate != ExpressListMenuCate.ToOffShelfSchedule
            && expressListMenuCate != ExpressListMenuCate.NotPickedLongDumpSchedule
            && expressListMenuCate != ExpressListMenuCate.NotOrderLongDumpSchedule) {
            throw new ValidationException("该方法当前只支持排班获取货架列表！");
        }

        queryParam.ScheduleShelfIdListQuery = null;

        ICriteria criteria = GetListCriteria(expressListMenuCate, queryParam);
        criteria = criteria.SetReadOnly(true);

        // string storageLocAlias = nameof(ExpressInfo.StorageLoc);
        criteria = criteria.SetProjection(Projections.Distinct(Projections.Property($"{nameof(ExpressInfo.StorageLoc)}.{nameof(ExpressInfo.StorageLoc.Shelf)}")));

        return criteria.List<ShelfInfo>().OrderBy(e => e.Name).ToList();
    }

    /// <summary>
    /// 获取查询 criteria
    /// </summary>
    /// <param name="expressListMenuCate"></param>        
    /// <param name="queryParam"></param>
    /// <returns></returns>
    private ICriteria GetListCriteria(ExpressListMenuCate expressListMenuCate, ExpressListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<ExpressInfo>();
        criteria = criteria.SetReadOnly(true);

        /* 不用关联表模式，性能太差
        criteria = criteria.CreateAlias(nameof(ExpressInfo.Warehouse), nameof(ExpressInfo.Warehouse));

        string storageLocAlias = nameof(ExpressInfo.StorageLoc);
        criteria = criteria.CreateAlias(storageLocAlias, storageLocAlias, JoinType.LeftOuterJoin);

        string shelfAlias = $"{nameof(ExpressInfo.StorageLoc)}.{nameof(ExpressInfo.StorageLoc.Shelf)}";
        criteria = criteria.CreateAlias(shelfAlias, shelfAlias, JoinType.LeftOuterJoin);

        string shelfSpecAlias = $"{nameof(ExpressInfo.StorageLoc)}.{nameof(ExpressInfo.StorageLoc.Spec)}";
        criteria = criteria.CreateAlias(shelfSpecAlias, shelfSpecAlias, JoinType.LeftOuterJoin);

        criteria = criteria.CreateAlias($"{nameof(ExpressInfo.Order)}", "order", JoinType.LeftOuterJoin);
        */

        var expressIds = queryParam.IdList;
        if (string.IsNullOrWhiteSpace(expressIds) == false) {
            List<int> expressIdList = new List<int>();
            IList<string> expressIdStrList = expressIds.Split(",", StringSplitOptions.RemoveEmptyEntries);
            foreach (string expressIdStr in expressIdStrList) {
                if (int.TryParse(expressIdStr, out int expressId)) {
                    expressIdList.Add(expressId);
                }
            }

            string expressIdProp = $"{nameof(ExpressInfo.Id)}";
            criteria = criteria.Add(Restrictions.In(expressIdProp, expressIdList));
        }

        string claimOrderIdProp = $"{nameof(ExpressInfo.Order)}.{nameof(LogisticsOrderInfo.Id)}";

        // 待下架
        if (expressListMenuCate == ExpressListMenuCate.ToOffShelf) {
            // criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.WaitPack && e.ArchiveType == ExpressArchiveType.None));
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.WaitPack)); // 有归档属性的快递单号也应该正常待下架显示
        }

        // 待下架排班
        if (expressListMenuCate == ExpressListMenuCate.ToOffShelfSchedule) {
            DetachedCriteria scheduleCriteria = DetachedCriteria.For<ExpressShelfScheduleInfo>();
            scheduleCriteria = scheduleCriteria.CreateAlias(nameof(ExpressShelfScheduleInfo.Express), nameof(ExpressShelfScheduleInfo.Express));
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.ScheduleType)}", ExpressShelfScheduleType.OffShelf));
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.Express)}.{nameof(ExpressShelfScheduleInfo.Express.Status)}", ExpressStatus.WaitPack)); // 缩小范围
            scheduleCriteria.SetProjection(Projections.Property($"{nameof(ExpressShelfScheduleInfo.Express)}.{nameof(ExpressShelfScheduleInfo.Express.Id)}"));

            criteria = criteria.Add(Restrictions.And(
                    // Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.WaitPack && e.ArchiveType == ExpressArchiveType.None),
                    Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.WaitPack), // 有归档属性的快递单号也应该正常下架排班显示
                    Subqueries.PropertyNotIn($"{nameof(ExpressInfo.Id)}", scheduleCriteria)
                )
            );
        }

        if (expressListMenuCate == ExpressListMenuCate.ToOffShelf || expressListMenuCate == ExpressListMenuCate.ToOffShelfSchedule) {
            if (queryParam.FirstModeIdQuery != 0) {
                int firstModeIdQuery = queryParam.FirstModeIdQuery;
                criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressInfo.FirstMode)}.{nameof(ExpressInfo.FirstMode.Id)}", firstModeIdQuery));
            }

            if (queryParam.SubmitPackTimeBeginQuery != null) {
                DateTime submitPackTimeBeginQuery = queryParam.SubmitPackTimeBeginQuery.Value.Date;
                criteria = criteria.Add(Restrictions.Ge($"{nameof(ExpressInfo.SubmitPackTime)}", submitPackTimeBeginQuery));
            }

            if (queryParam.SubmitPackTimeEndQuery != null) {
                DateTime submitPackTimeEndQuery = queryParam.SubmitPackTimeEndQuery.Value.Date.AddDays(1);
                criteria = criteria.Add(Restrictions.Lt($"{nameof(ExpressInfo.SubmitPackTime)}", submitPackTimeEndQuery));
            }
        }

        // 按界面分类过滤

        if (queryParam.WarehouseIdQuery != 0) {
            int warehouseIdQuery = queryParam.WarehouseIdQuery;
            criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressInfo.Warehouse)}.{nameof(ExpressInfo.Warehouse.Id)}", warehouseIdQuery));
        }

        IList<ExpressStatus> mayToOnShelfStatusList = ExpressStatus.GetMayToOnShelfList();

        // 待上架
        if (expressListMenuCate == ExpressListMenuCate.ToOnShelf) {
            /*
            IList<int> groundShelfIdList = session.Query<ShelfInfo>().Where(e => e.Cate == ShelfCate.Ground).Select(e => e.Id).ToList();
            criteria = criteria.Add(Restrictions.Not(Restrictions.In($"{shelfAlias}.{nameof(ShelfInfo.Id)}", groundShelfIdList.ToArray()))); // 待上架和待上架排班都不显示地面仓位
            */

            IList<int> groundStorageLocIdList = session.Query<StorageLocInfo>().Where(e => e.Shelf.Cate == ShelfCate.Ground).Select(e => e.Id).ToList(); // 找出所有地面仓位，应该不会很多
            criteria = criteria.Add(Restrictions.Not(Restrictions.In($"{nameof(ExpressInfo.StorageLoc)}.{nameof(StorageLocInfo.Id)}", groundStorageLocIdList.ToArray()))); // 待上架和待上架排班都不显示地面仓位                

            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.ShelfStatus == ExpressShelfStatus.ToOnShelf && e.ArchiveType == ExpressArchiveType.None));
            criteria = criteria.Add(Restrictions.In(nameof(ExpressInfo.Status), mayToOnShelfStatusList.ToArray()));
        }

        // 待上架排班
        if (expressListMenuCate == ExpressListMenuCate.ToOnShelfSchedule) {
            /*
            IList<int> groundShelfIdList = session.Query<ShelfInfo>().Where(e => e.Cate == ShelfCate.Ground).Select(e => e.Id).ToList();
            criteria = criteria.Add(Restrictions.Not(Restrictions.In($"{shelfAlias}.{nameof(ShelfInfo.Id)}", groundShelfIdList.ToArray()))); // 待上架和待上架排班都不显示地面仓位
            */

            IList<int> groundStorageLocIdList = session.Query<StorageLocInfo>().Where(e => e.Shelf.Cate == ShelfCate.Ground).Select(e => e.Id).ToList(); // 找出所有地面仓位，应该不会很多
            criteria = criteria.Add(Restrictions.Not(Restrictions.In($"{nameof(ExpressInfo.StorageLoc)}.{nameof(StorageLocInfo.Id)}", groundStorageLocIdList.ToArray()))); // 待上架和待上架排班都不显示地面仓位                  

            DetachedCriteria scheduleCriteria = DetachedCriteria.For<ExpressShelfScheduleInfo>();
            scheduleCriteria = scheduleCriteria.CreateAlias(nameof(ExpressShelfScheduleInfo.Express), nameof(ExpressShelfScheduleInfo.Express));
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.ScheduleType)}", ExpressShelfScheduleType.OnShelf));
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.Express)}.{nameof(ExpressShelfScheduleInfo.Express.ShelfStatus)}", ExpressShelfStatus.ToOnShelf));
            scheduleCriteria.SetProjection(Projections.Property($"{nameof(ExpressShelfScheduleInfo.Express)}.{nameof(ExpressShelfScheduleInfo.Express.Id)}"));

            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.ShelfStatus == ExpressShelfStatus.ToOnShelf && e.ArchiveType == ExpressArchiveType.None));
            criteria = criteria.Add(Restrictions.In(nameof(ExpressInfo.Status), mayToOnShelfStatusList.ToArray()));
            criteria = criteria.Add(Subqueries.PropertyNotIn($"{nameof(ExpressInfo.Id)}", scheduleCriteria));
        }

        // 已上架
        if (expressListMenuCate == ExpressListMenuCate.OnShelf) {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                e.AddTime >= EXPRESS_CHECKIN_RECENT_AGO && e.ShelfStatus == ExpressShelfStatus.OnShelf));
        }

        // 已认领
        if (expressListMenuCate == ExpressListMenuCate.Picked) {
            DetachedCriteria expectExpressCriteria = DetachedCriteria.For<ExpressExpectInfo>()
                .Add(Restrictions.Where<ExpressExpectInfo>(e => e.Status == ExpressExpectStatus.Create || e.Status == ExpressExpectStatus.Enter)) // create 状态放在这里只是为了容错
                .SetProjection(Projections.Property<ExpressExpectInfo>(e => e.TrackingNo));

            criteria = criteria.Add(Restrictions.And(
                    Restrictions.Where<ExpressInfo>(e => e.AddTime >= EXPRESS_CHECKIN_RECENT_AGO),
                    Restrictions.Or(
                        Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.Checkin),
                        Subqueries.PropertyIn(nameof(ExpressInfo.TrackingNo), expectExpressCriteria)
                    )
                )
            );
        }

        // 未认领
        if (expressListMenuCate == ExpressListMenuCate.NotPicked) {
            DetachedCriteria expectExpressCriteria = DetachedCriteria.For<ExpressExpectInfo>()
                .Add(Restrictions.Where<ExpressExpectInfo>(e => e.Status == ExpressExpectStatus.Create || e.Status == ExpressExpectStatus.Enter)) // create 状态放在这里只是为了容错
                .SetProjection(Projections.Property<ExpressExpectInfo>(e => e.TrackingNo));

            criteria = criteria.Add(Restrictions.And(
                    Restrictions.Where<ExpressInfo>(e => e.AddTime >= EXPRESS_CHECKIN_RECENT_AGO && e.Status == ExpressStatus.WaitPick),
                    Restrictions.Not(Subqueries.PropertyIn(nameof(ExpressInfo.TrackingNo), expectExpressCriteria))
                )
            );
        }

        // 已预报
        if (expressListMenuCate == ExpressListMenuCate.Expect) {
            DateTime beforeRecentAgo = EXPRESS_CHECKIN_RECENT_AGO.AddDays(-20); // DateTime.Today.AddDays(-110); // 110天之前的时间段，做为 90 天内预报的起始点，提前20天可能的预报应该够了

            DetachedCriteria expectExpressCriteria = DetachedCriteria.For<ExpressExpectInfo>()
                // .Add(Restrictions.Where<ExpressExpectInfo>(e => e.Status == ExpressExpectStatus.Enter)) // 如果这里没有状态限制，到时就加个时间限制，比如180天内添加的
                .Add(Restrictions.Where<ExpressExpectInfo>(e => e.UpdateTime >= beforeRecentAgo))
                .SetProjection(Projections.Property<ExpressExpectInfo>(e => e.TrackingNo));

            criteria = criteria.Add(Restrictions.And(
                Restrictions.Where<ExpressInfo>(e =>
                    e.AddTime >= EXPRESS_CHECKIN_RECENT_AGO // 90 天
                    // && (e.Status == ExpressStatus.WaitPick || e.Status == ExpressStatus.CheckIn || e.Status == ExpressStatus.WaitOrder)),
                    && e.Status == ExpressStatus.WaitPick),
                Subqueries.PropertyIn(nameof(ExpressInfo.TrackingNo), expectExpressCriteria))
            );
        }

        // 长期未认领
        if (expressListMenuCate == ExpressListMenuCate.NotPickedLong) {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                e.AddTime < EXPRESS_CHECKIN_RECENT_AGO
                && e.Status == ExpressStatus.WaitPick
                && e.DumpStatus == ExpressDumpStatus.NotDump
                && e.ArchiveType == ExpressArchiveType.None)
            );
        }

        // 长期未出库
        if (expressListMenuCate == ExpressListMenuCate.NotOrderLong) {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                e.AddTime < EXPRESS_CHECKIN_RECENT_AGO
                && e.Status == ExpressStatus.Checkin
                && e.DumpStatus == ExpressDumpStatus.NotDump
                && e.ArchiveType == ExpressArchiveType.None)
            );
        }

        // 长期未认领堆存排班
        if (expressListMenuCate == ExpressListMenuCate.NotPickedLongDumpSchedule) {
            DetachedCriteria scheduleCriteria = DetachedCriteria.For<ExpressDumpScheduleInfo>();
            scheduleCriteria = scheduleCriteria.CreateAlias(nameof(ExpressDumpScheduleInfo.Express), nameof(ExpressDumpScheduleInfo.Express));
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressDumpScheduleInfo.DumpType)}", ExpressDumpType.NoPickup));
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressDumpScheduleInfo.Express)}.{nameof(ExpressDumpScheduleInfo.Express.DumpStatus)}", ExpressDumpStatus.NotDump));
            scheduleCriteria.SetProjection(Projections.Property($"{nameof(ExpressDumpScheduleInfo.Express)}.{nameof(ExpressDumpScheduleInfo.Express.Id)}"));

            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                e.AddTime < EXPRESS_CHECKIN_RECENT_AGO
                && e.Status == ExpressStatus.WaitPick
                && e.DumpStatus == ExpressDumpStatus.NotDump
                && e.ArchiveType == ExpressArchiveType.None));

            criteria = criteria.Add(Subqueries.PropertyNotIn($"{nameof(ExpressInfo.Id)}", scheduleCriteria));
        }

        // 长期未出库堆存排班
        if (expressListMenuCate == ExpressListMenuCate.NotOrderLongDumpSchedule) {
            DetachedCriteria scheduleCriteria = DetachedCriteria.For<ExpressDumpScheduleInfo>();
            scheduleCriteria = scheduleCriteria.CreateAlias(nameof(ExpressDumpScheduleInfo.Express), nameof(ExpressDumpScheduleInfo.Express));
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressDumpScheduleInfo.DumpType)}", ExpressDumpType.NoProcess));
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressDumpScheduleInfo.Express)}.{nameof(ExpressDumpScheduleInfo.Express.DumpStatus)}", ExpressDumpStatus.NotDump));
            scheduleCriteria.SetProjection(Projections.Property($"{nameof(ExpressDumpScheduleInfo.Express)}.{nameof(ExpressDumpScheduleInfo.Express.Id)}"));

            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                e.AddTime < EXPRESS_CHECKIN_RECENT_AGO
                && e.Status == ExpressStatus.Checkin
                && e.DumpStatus == ExpressDumpStatus.NotDump
                && e.ArchiveType == ExpressArchiveType.None));

            criteria = criteria.Add(Subqueries.PropertyNotIn($"{nameof(ExpressInfo.Id)}", scheduleCriteria));
        }

        // 已堆存
        if (expressListMenuCate == ExpressListMenuCate.Dumped) {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.DumpStatus == ExpressDumpStatus.Dumped));
        }

        // 历史归档
        if (expressListMenuCate == ExpressListMenuCate.Archive) {
            if (queryParam.ArchiveTypeQuery == null) {
                queryParam.ArchiveTypeQuery = ExpressArchiveType.None;
            }

            if (queryParam.ArchiveTypeQuery == ExpressArchiveType.None) {
                criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                    e.ArchiveType == ExpressArchiveType.ToOnShelf
                    || e.ArchiveType == ExpressArchiveType.ToOffShelf
                    || ((e.Status == ExpressStatus.WaitPick && e.ArchiveType == ExpressArchiveType.None) || e.ArchiveType == ExpressArchiveType.NotPickedLong)
                    || ((e.Status == ExpressStatus.Checkin && e.ArchiveType == ExpressArchiveType.None) || e.ArchiveType == ExpressArchiveType.NotOrderLong)
                ));
            }

            if (queryParam.ArchiveTypeQuery == ExpressArchiveType.ToOnShelf) {
                criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                    e.ArchiveType == ExpressArchiveType.ToOnShelf
                ));
            }

            if (queryParam.ArchiveTypeQuery == ExpressArchiveType.ToOffShelf) {
                criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                    e.ArchiveType == ExpressArchiveType.ToOffShelf
                ));
            }

            if (queryParam.ArchiveTypeQuery == ExpressArchiveType.NotPickedLong) {
                criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                    ((e.Status == ExpressStatus.WaitPick && e.ArchiveType == ExpressArchiveType.None) || e.ArchiveType == ExpressArchiveType.NotPickedLong)
                ));
            }

            if (queryParam.ArchiveTypeQuery == ExpressArchiveType.NotOrderLong) {
                criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                    ((e.Status == ExpressStatus.Checkin && e.ArchiveType == ExpressArchiveType.None) || e.ArchiveType == ExpressArchiveType.NotOrderLong)
                ));
            }
        }

        // 已下架
        if (expressListMenuCate == ExpressListMenuCate.OffShelf) {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.OffShelf));
        }

        // 打包中
        if (expressListMenuCate == ExpressListMenuCate.Packing) {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.Packing));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CheckinTrackingNoQuery) == false) {
            string checkinTrackingNoQuery = queryParam.CheckinTrackingNoQuery.Trim();
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.CheckinTrackingNo == checkinTrackingNoQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.TrackingNoQuery) == false) {
            string trackingNoQuery = queryParam.TrackingNoQuery.Trim();
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.TrackingNo == trackingNoQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.BillCustomerCodeQuery) == false) {
            string billCustomerCodeQuery = queryParam.BillCustomerCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.BillCustomerCode == billCustomerCodeQuery));
        }

        if (queryParam.HasBillCustomerIdQuery != null) {
            if (queryParam.HasBillCustomerIdQuery.Value == false) // 解析失败的
            {
                criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.BillCustomerId == 0));
            } else // 解析成功的
            {
                criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.BillCustomerId != 0));
            }
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false) {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery.Trim());
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Customer!.Id == customerIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false) {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            LogisticsOrderInfo? order = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCodeQuery)
                .Take(1)
                .SingleOrDefault();

            int orderId = order?.Id ?? 0;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Order!.Id == orderId));
        }

        if (string.IsNullOrWhiteSpace(queryParam.ExpressCompanyQuery) == false) {
            string expressCompanyQuery = queryParam.ExpressCompanyQuery.Trim();
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.ExpressCompany == expressCompanyQuery));
        }

        if (queryParam.StatusQuery != null && queryParam.StatusQuery != ExpressStatus.None) {
            ExpressStatus statusQuery = queryParam.StatusQuery;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Status == statusQuery));
        }

        if (queryParam.ShelfStatusQuery != null && queryParam.ShelfStatusQuery != ExpressShelfStatus.None) {
            ExpressShelfStatus shelfStatusQuery = queryParam.ShelfStatusQuery;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.ShelfStatus == shelfStatusQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.GoodsNameQuery) == false) {
            string goodsNameQuery = queryParam.GoodsNameQuery.Trim();
            IList<int> goodsIdList = session.Query<LogisticsGoodsInfo>()
                .Where(e => e.Name.Contains(goodsNameQuery))
                .Select(e => e.Id)
                .ToList();

            IList<int> expressIdList = session.Query<ExpressGoodsInfo>()
                .Where(e => goodsIdList.Contains(e.Goods.Id))
                .Select(e => e.Express.Id)
                .Distinct()
                .ToList();

            criteria = criteria.Add(Restrictions.On<ExpressInfo>(e => e.Id).IsIn(expressIdList.ToArray()));
        }

        if (queryParam.AddDateBeginQuery != null) {
            DateTime addDateBeginQueryValue = queryParam.AddDateBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.AddTime >= addDateBeginQueryValue));
        }

        if (queryParam.AddDateEndQuery != null) {
            DateTime addDateEndQueryValue = queryParam.AddDateEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.AddTime < addDateEndQueryValue));
        }

        if (queryParam.AddTimeBeginQuery != null) {
            DateTime addTimeBeginQueryValue = queryParam.AddTimeBeginQuery.Value;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.AddTime >= addTimeBeginQueryValue));
        }

        if (queryParam.AddTimeEndQuery != null) {
            DateTime addTimeEndQueryValue = queryParam.AddTimeEndQuery.Value;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.AddTime <= addTimeEndQueryValue));
        }

        if (queryParam.OnShelfTimeBeginQuery != null) {
            DateTime OnShelfTimeBeginQueryValue = queryParam.OnShelfTimeBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.OnShelfTime >= OnShelfTimeBeginQueryValue));
        }

        if (queryParam.OnShelfTimeEndQuery != null) {
            DateTime OnShelfTimeEndQueryValue = queryParam.OnShelfTimeEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.OnShelfTime < OnShelfTimeEndQueryValue));
        }

        if (queryParam.ClaimTimeBeginQuery != null) {
            DateTime pickTimeBeginQueryValue = queryParam.ClaimTimeBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.ClaimTime >= pickTimeBeginQueryValue));
        }

        if (queryParam.ClaimTimeEndQuery != null) {
            DateTime pickTimeEndQueryValue = queryParam.ClaimTimeEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.ClaimTime < pickTimeEndQueryValue));
        }

        if (queryParam.DumpTimeBeginQuery != null) {
            DateTime dumpTimeBeginQueryValue = queryParam.DumpTimeBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.DumpTime >= dumpTimeBeginQueryValue));
        }

        if (queryParam.DumpTimeEndQuery != null) {
            DateTime dumpTimeEndQueryValue = queryParam.DumpTimeEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.DumpTime < dumpTimeEndQueryValue));
        }

        if (queryParam.ScheduleShelfIdListQuery != null && queryParam.ScheduleShelfIdListQuery.Count != 0) {
            /*
            string shelfIdProp = $"{shelfAlias}.{nameof(ExpressInfo.StorageLoc.Shelf.Id)}";
            criteria = criteria.Add(Restrictions.In(shelfIdProp, queryParam.ScheduleShelfIdListQuery.ToArray()));
            */

            var shelfIdListQuery = queryParam.ScheduleShelfIdListQuery;
            var shelfStorageLocIdList = session.Query<StorageLocInfo>().Where(e => shelfIdListQuery.Contains(e.Shelf.Id)).Select(e => e.Id).ToList();
            criteria = criteria.Add(Restrictions.In($"{nameof(ExpressInfo.StorageLoc)}.{nameof(StorageLocInfo.Id)}", shelfStorageLocIdList));
        }

        if (queryParam.ExpectedQuery != null) {
            bool expectedQueryValue = queryParam.ExpectedQuery.Value;


            if (expectedQueryValue == true) {
                DetachedCriteria expectExpressCriteria = DetachedCriteria.For<ExpressExpectInfo>()
                    .Add(Restrictions.Or(
                        Restrictions.Eq(nameof(ExpressExpectInfo.Status), ExpressExpectStatus.Create),
                        Restrictions.Eq(nameof(ExpressExpectInfo.Status), ExpressExpectStatus.Enter))
                    )
                    .SetProjection(Projections.Property<ExpressExpectInfo>(e => e.TrackingNo));

                criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.WaitPick));

                criteria = criteria.Add(Subqueries.PropertyIn(nameof(ExpressInfo.TrackingNo), expectExpressCriteria));
            } else {
                DetachedCriteria expectExpressCriteria = DetachedCriteria.For<ExpressExpectInfo>()
                    .SetProjection(Projections.Property<ExpressExpectInfo>(e => e.TrackingNo));

                criteria = criteria.Add(Subqueries.PropertyNotIn(nameof(ExpressInfo.TrackingNo), expectExpressCriteria));
            }
        }

        if (queryParam.CheckinUserIdQuery != null) {
            int checkinUserIdQuery = queryParam.CheckinUserIdQuery.Value;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.AddUser.Id == checkinUserIdQuery));
        }

        return criteria;
    }
}
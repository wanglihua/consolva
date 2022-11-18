using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Domain.Storage;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;
using Order = NHibernate.Criterion.Order;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressClaimService
{
    /// <summary>
    /// 上架和上架排班的只考虑这个时间后的
    /// </summary>
    public static DateTime EXPRESS_ON_SHELF_AFTER => new DateTime(2021, 8, 19); // 2021-08-19

    // 这里采用 => 只读属性的目的就是防止值被修改，不仅能防止被修改，而且还能保证每次取这个值时都会按现在时间重新计算
    // 所以必须要用箭头
    // public static DateTime EXPRESS_CHECKIN_RECENT_AGO => DateTime.Today.AddDays(-90); // 90天之前的时间段，做为长期和近期的区分点

    public static DateTime EXPRESS_CHECKIN_RECENT_AGO // 还是改成 get 的方式比较放心
    {
        get
        {
            return DateTime.Today.AddDays(-90); // 90天之前的时间段，做为长期和近期的区分点          
        }
    }

    // 3.0 系统启用的时间点
    public static DateTime ARCHIVE_DATE => new DateTime(2021, 5, 1);

    private ISession session;
    private ExpressSizeWeightService expressSizeWeightService;
    private CustomerCodeService customerCodeService;
    private ExpressClaimImageService expressClaimImageService;

    public ExpressClaimService(ISession session,
        ExpressSizeWeightService expressSizeWeightService,
        CustomerCodeService customerCodeService,
        ExpressClaimImageService expressClaimImageService)
    {
        this.session = session;
        this.expressSizeWeightService = expressSizeWeightService;
        this.customerCodeService = customerCodeService;
        this.expressClaimImageService = expressClaimImageService;
    }

    /// <summary>
    /// 分页获取列表数据
    /// </summary>
    /// <param name="expressClaimCate"></param>
    /// <param name="queryParam"></param>
    /// <param name="listPager"></param>
    /// <returns></returns>
    public IList<ExpressInfo> List(ExpressClaimCate expressClaimCate, ExpressClaimListQueryParam queryParam, ListPager listPager)
    {
        ICriteria criteria = GetListCriteria(expressClaimCate, queryParam);

        if (listPager != null)
        {
            criteria = criteria.AddOrder(new Order(listPager.SortField, listPager.IsAscending))
                .AddOrder(Order.Desc(nameof(Entity.Id)))
                .SetFirstResult(listPager.PageIndex * listPager.PageSize)
                .SetMaxResults(listPager.PageSize);
        }

        IList<ExpressInfo> claimGoodsList = criteria.List<ExpressInfo>();

        List<int> userIdList = claimGoodsList.Select(e => e.AddUser.Id).Distinct().ToList();
        userIdList.AddRange(claimGoodsList.Where(e => e.OnShelfUser != null).Select(e => e.OnShelfUser.Id).Distinct().ToList());
        userIdList.AddRange(claimGoodsList.Where(e => e.OffShelfUser != null).Select(e => e.OffShelfUser.Id).Distinct().ToList());
        userIdList.AddRange(claimGoodsList.Where(e => e.PackingUser != null).Select(e => e.PackingUser.Id).Distinct().ToList());
        userIdList.AddRange(claimGoodsList.Where(e => e.DumpUser != null).Select(e => e.DumpUser.Id).Distinct().ToList());
        userIdList = userIdList.Distinct().ToList();

        IList<int> customerIdList = claimGoodsList.Where(e => e.Customer != null).Select(e => e.Customer!.Id).Distinct().ToList();
        IList<int> orderIdList = claimGoodsList.Where(e => e.Order != null).Select(e => e.Order.Id).Distinct().ToList();

        List<int> storageLocIdList = claimGoodsList.Select(e => e.StorageLoc.Id).Distinct().ToList();
        // storageLocIdList.AddRange(claimGoodsList.Where(e => e.CheckinStorageLoc != null).Select(e => e.CheckinStorageLoc.Id).Distinct().ToList());
        storageLocIdList = storageLocIdList.Distinct().ToList();

        IList<UserInfo> userList = session.Query<UserInfo>().Where(e => e.LoginName != UserInfo.DevLoginName && userIdList.Contains(e.Id)).ToList();
        IList<CustomerInfo> customerList = session.Query<CustomerInfo>().Where(e => customerIdList.Contains(e.Id)).ToList();
        IList<LogisticsOrderInfo> orderList = session.Query<LogisticsOrderInfo>().Where(e => orderIdList.Contains(e.Id)).ToList();
        IList<StorageLocInfo> storageLocList = session.Query<StorageLocInfo>().Fetch(e => e.Spec).Fetch(e => e.Shelf).Where(e => storageLocIdList.Contains(e.Id)).ToList();
        IList<WarehouseInfo> warehouseList = session.Query<WarehouseInfo>().ToList(); // 所有的吧，总共也没多少。有些仓库，当前是禁用了，但之前存储的确实是这个禁用的值。
        IList<LogisticsModeInfo> modeList = session.Query<LogisticsModeInfo>()
            .Where(e => e.Status == LogisticsModeStatus.Enabled || e.InnerStatus == LogisticsModeStatus.Enabled) // 只取当前活跃的物流方式
            .ToList();

        IList<string> trackingNoList = claimGoodsList.Select(e => e.TrackingNo).ToList();
        IList<ExpressSizeWeightInfo> expressItemList = expressSizeWeightService.GetExpressSizeWeightList(trackingNoList);

        IList<ExpressExpectInfo> expressExpectList = session.Query<ExpressExpectInfo>()
            .Fetch(e => e.Customer)
            .Where(e => trackingNoList.Contains(e.TrackingNo))
            .ToList();

        IList<LogisticsOrderPhotoInfo> expressPhotoList = expressClaimImageService.GetExpressPhotoList(trackingNoList);

        foreach (ExpressInfo claimGoods in claimGoodsList)
        {
            UserInfo addUser = userList.Where(e => e.Id == claimGoods.AddUser.Id).Take(1).SingleOrDefault();
            claimGoods.AddUserNameCache = addUser?.UserName;

            // on shelf user, off shelf user，packing user 不采用关联表的方式
            int onShelfUserId = claimGoods.OnShelfUser?.Id ?? 0;
            UserInfo onShelfUser = userList.Where(e => e.Id == onShelfUserId).Take(1).SingleOrDefault();
            claimGoods.OnShelfUserNameCache = onShelfUser?.UserName;

            int offShelfUserId = claimGoods.OffShelfUser?.Id ?? 0;
            UserInfo offShelfUser = userList.Where(e => e.Id == offShelfUserId).Take(1).SingleOrDefault();
            claimGoods.OffShelfUserNameCache = offShelfUser?.UserName;

            int packingUserId = claimGoods.PackingUser?.Id ?? 0;
            UserInfo packingUser = userList.Where(e => e.Id == packingUserId).Take(1).SingleOrDefault();
            claimGoods.PackingUserNamCache = packingUser?.UserName;

            int dumpUserId = claimGoods.DumpUser?.Id ?? 0;
            UserInfo dumpUser = userList.Where(e => e.Id == dumpUserId).Take(1).SingleOrDefault();
            claimGoods.DumpUserNameCache = dumpUser?.UserName;

            CustomerInfo customer = customerList.Where(e => e.Id == claimGoods.CustomerId).Take(1).SingleOrDefault();
            claimGoods.CustomerNameCache = customer?.CustomerCode;

            LogisticsOrderInfo order = orderList.Where(e => e.Id == (claimGoods.Order?.Id ?? 0)).Take(1).SingleOrDefault();
            claimGoods.OrderCodeCache = order?.Code;
            claimGoods.OrderStatusNameCache = order?.OrderStatus?.Name;
            claimGoods.orderTargetWarehouseNameCache = warehouseList.Where(e => e.Id == (order?.TargetWarehouse?.Id ?? 0)).Take(1).SingleOrDefault()?.Name;
            claimGoods.OrderFirstModeNameCache = modeList.Where(e => e.Id == (order?.FirstLogisticsMode?.Id ?? 0)).Take(1).SingleOrDefault()?.Name;
            claimGoods.OrderSubmitPackTimeCache = order?.SubmitPackTime;

            StorageLocInfo storageLoc = storageLocList.Where(e => e.Id == (claimGoods.StorageLoc?.Id ?? 0)).Take(1).SingleOrDefault();
            claimGoods.StorageLocNameCache = storageLoc?.GetFullName();

            WarehouseInfo checkinWarehouse = warehouseList.Where(e => e.Id == (claimGoods.CheckinWarehouse?.Id ?? 0)).Take(1).SingleOrDefault();
            claimGoods.CheckinWarehouseNameCache = checkinWarehouse?.Name;

            WarehouseInfo warehouse = warehouseList.Where(e => e.Id == (claimGoods.Warehouse?.Id ?? 0)).Take(1).SingleOrDefault();
            claimGoods.WarehouseNameCache = warehouse?.Name;

            StorageLocInfo checkinStorageLoc = storageLocList.Where(e => e.Id == (claimGoods.CheckinStorageLoc?.Id ?? 0)).Take(1).SingleOrDefault();
            claimGoods.CheckinStorageLocNameCache = checkinStorageLoc?.GetFullName();

            claimGoods.ExpressItemListCache = expressItemList.Where(e => e.ExpressTrackingNo == claimGoods.TrackingNo).ToList();

            ExpressExpectInfo expect = expressExpectList.Where(e => e.TrackingNo == claimGoods.TrackingNo).Take(1).SingleOrDefault();
            claimGoods.ExpectCustomerIdCache = expect?.Customer?.CustomerCode ?? string.Empty;
            claimGoods.ExpectTimeCache = expect?.CreateTime;

            claimGoods.ExpressPhotoListCache = expressPhotoList
                .Where(e => e.ExpressTrackingNo == claimGoods.TrackingNo)
                .OrderBy(e => e.Type.Id) // 必须要用 Type.Id, Type之间还不能比较
                .ToList();
        }

        return claimGoodsList;
    }

    /// <summary>
    /// 不分页获取列表数据
    /// </summary>
    /// <param name="expressClaimCate"></param>
    /// <param name="queryParam"></param>
    /// <returns></returns>
    public IList<ExpressInfo> List(ExpressClaimCate expressClaimCate, ExpressClaimListQueryParam queryParam)
    {
        return List(expressClaimCate, queryParam, null);
    }

    /// <summary>
    /// 计数
    /// </summary>
    /// <param name="expressClaimCate"></param>
    /// <param name="queryParam"></param>
    /// <returns></returns>
    public int Count(ExpressClaimCate expressClaimCate, ExpressClaimListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(expressClaimCate, queryParam)
            .SetProjection(Projections.Count<ExpressInfo>(e => e.Id))
            .UniqueResult<int>();

        return totalCount;
    }

    public IList<ShelfInfo> GetShelfList(ExpressClaimCate expressClaimCate, int warehouseId)
    {
        if (expressClaimCate != ExpressClaimCate.ToOnShelfSchedule
            && expressClaimCate != ExpressClaimCate.ToOffShelfSchedule
            && expressClaimCate != ExpressClaimCate.NotPickedLongDumpSchedule
            && expressClaimCate != ExpressClaimCate.NotOrderLongDumpSchedule)
        {
            throw new ValidationException("该方法当前只支持排班获取货架列表！");
        }

        ICriteria criteria = session.CreateCriteria<ExpressInfo>();
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(ExpressInfo.StorageLoc), "sl");

        criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressInfo.Warehouse)}.{nameof(ExpressInfo.Warehouse.Id)}", warehouseId));

        // 待上架排班
        if (expressClaimCate == ExpressClaimCate.ToOnShelfSchedule)
        {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.ShelfStatus == ExpressShelfStatus.ToOnShelf && e.AddTime >= EXPRESS_ON_SHELF_AFTER));
        }

        // 待下架排班
        if (expressClaimCate == ExpressClaimCate.ToOffShelfSchedule)
        {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.WaitPack));
        }

        // 长期未认领待堆存排班
        if (expressClaimCate == ExpressClaimCate.NotPickedLongDumpSchedule)
        {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                e.AddTime >= ARCHIVE_DATE
                && e.AddTime < EXPRESS_CHECKIN_RECENT_AGO
                && e.Status == ExpressStatus.WaitPick
                && e.DumpStatus == ExpressDumpStatus.NotDump));
        }

        // 长期未出库待堆存排班
        if (expressClaimCate == ExpressClaimCate.NotOrderLongDumpSchedule)
        {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                e.AddTime >= ARCHIVE_DATE
                && e.AddTime < EXPRESS_CHECKIN_RECENT_AGO
                && (e.Status == ExpressStatus.CheckIn || e.Status == ExpressStatus.WaitOrder)
                && e.DumpStatus == ExpressDumpStatus.NotDump));
        }

        // criteria = criteria.AddOrder(Order.Asc($"{nameof(ExpressInfo.StorageLoc)}.{nameof(ExpressInfo.StorageLoc.Shelf)}.{nameof(ExpressInfo.StorageLoc.Shelf.Name)}"));

        criteria = criteria.SetProjection(Projections.Distinct(Projections.Property($"sl.{nameof(ExpressInfo.StorageLoc.Shelf)}")));

        return criteria.List<ShelfInfo>().OrderBy(e => e.Name).ToList();
    }

    public IList<ShelfInfo> GetShelfList(ExpressClaimCate expressClaimCate, ExpressClaimListQueryParam queryParam)
    {
        if (expressClaimCate != ExpressClaimCate.ToOnShelfSchedule
            && expressClaimCate != ExpressClaimCate.ToOffShelfSchedule
            && expressClaimCate != ExpressClaimCate.NotPickedLongDumpSchedule
            && expressClaimCate != ExpressClaimCate.NotOrderLongDumpSchedule)
        {
            throw new ValidationException("该方法当前只支持排班获取货架列表！");
        }

        queryParam.ScheduleShelfIdListQuery = null;

        ICriteria criteria = GetListCriteria(expressClaimCate, queryParam);
        criteria = criteria.SetReadOnly(true);

        // string storageLocAlias = nameof(ExpressInfo.StorageLoc);
        criteria = criteria.SetProjection(Projections.Distinct(Projections.Property($"{nameof(ExpressInfo.StorageLoc)}.{nameof(ExpressInfo.StorageLoc.Shelf)}")));

        return criteria.List<ShelfInfo>().OrderBy(e => e.Name).ToList();
    }

    /// <summary>
    /// 获取查询 criteria
    /// </summary>
    /// <param name="expressClaimCate"></param>        
    /// <param name="queryParam"></param>
    /// <returns></returns>
    private ICriteria GetListCriteria(ExpressClaimCate expressClaimCate, ExpressClaimListQueryParam queryParam)
    {
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

        var claimIds = queryParam.IdList;
        if (string.IsNullOrWhiteSpace(claimIds) == false)
        {
            List<int> claimIdList = new List<int>();
            IList<string> claimIdStrList = claimIds.Split(",", StringSplitOptions.RemoveEmptyEntries);
            foreach (string claimIdStr in claimIdStrList)
            {
                if (int.TryParse(claimIdStr, out int claimId))
                {
                    claimIdList.Add(claimId);
                }
            }

            string claimIdProp = $"{nameof(ExpressInfo.Id)}";
            criteria = criteria.Add(Restrictions.In(claimIdProp, claimIdList));
        }

        string claimOrderIdProp = $"{nameof(ExpressInfo.Order)}.{nameof(LogisticsOrderInfo.Id)}";

        // 待下架
        if (expressClaimCate == ExpressClaimCate.ToOffShelf)
        {
            // criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.WaitPack && e.ArchiveType == ExpressArchiveType.None));
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.WaitPack)); // 有归档属性的快递单号也应该正常待下架显示
        }

        // 待下架排班
        if (expressClaimCate == ExpressClaimCate.ToOffShelfSchedule)
        {
            DetachedCriteria scheduleCriteria = DetachedCriteria.For<ExpressShelfScheduleInfo>();
            scheduleCriteria = scheduleCriteria.CreateAlias(nameof(ExpressShelfScheduleInfo.Claim), nameof(ExpressShelfScheduleInfo.Claim));
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.ScheduleType)}", ExpressShelfScheduleType.OffShelf));
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.Claim)}.{nameof(ExpressShelfScheduleInfo.Claim.Status)}", ExpressStatus.WaitPack)); // 缩小范围
            scheduleCriteria.SetProjection(Projections.Property($"{nameof(ExpressShelfScheduleInfo.Claim)}.{nameof(ExpressShelfScheduleInfo.Claim.Id)}"));

            criteria = criteria.Add(Restrictions.And(
                    // Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.WaitPack && e.ArchiveType == ExpressArchiveType.None),
                    Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.WaitPack), // 有归档属性的快递单号也应该正常下架排班显示
                    Subqueries.PropertyNotIn($"{nameof(ExpressInfo.Id)}", scheduleCriteria)
                )
            );
        }

        if (expressClaimCate == ExpressClaimCate.ToOffShelf || expressClaimCate == ExpressClaimCate.ToOffShelfSchedule)
        {
            /* 不用关联表模式，性能太差
            criteria = criteria.CreateAlias($"order.{nameof(LogisticsOrderInfo.TargetWarehouse)}", "tw", JoinType.LeftOuterJoin);
            criteria = criteria.CreateAlias($"order.{nameof(LogisticsOrderInfo.FirstLogisticsMode)}", "fm", JoinType.LeftOuterJoin);
            
            criteria = criteria.Add(Restrictions.Eq($"order.{nameof(LogisticsOrderInfo.OrderStatus)}", LogisticsOrderStatus.CnWaitPack));                
            */

            // 直接把所有待待打包的订单全找出来，应该不会很多
            var orderWaitPackListQuery = session.Query<LogisticsOrderInfo>()
                .Where(e => e.OrderStatus == LogisticsOrderStatus.CnWaitPack);

            // 后面的查询就是缩小 orderWaitPackList 的范围 

            if (queryParam.OrderTargetWarehouseIdQuery != 0)
            {
                // criteria = criteria.Add(Restrictions.Eq($"tw.{nameof(WarehouseInfo.Id)}", queryParam.OrderTargetWarehouseIdQuery));

                int twIdQuery = queryParam.OrderTargetWarehouseIdQuery;
                orderWaitPackListQuery = orderWaitPackListQuery.Where(e => e.TargetWarehouse.Id == twIdQuery);
            }

            if (queryParam.FirstModeIdQuery != 0)
            {
                // criteria = criteria.Add(Restrictions.Eq($"fm.{nameof(LogisticsModeInfo.Id)}", queryParam.FirstModeIdQuery));

                int fmIdQuery = queryParam.FirstModeIdQuery;
                orderWaitPackListQuery = orderWaitPackListQuery.Where(e => e.FirstLogisticsMode.Id == fmIdQuery);
            }

            if (queryParam.SyncTlaQuery != null)
            {
                /*
                string syncTlaProp = $"fm.{nameof(LogisticsModeInfo.SyncTla)}";

                if (queryParam.SyncTlaQuery == false)
                {
                    // criteria = criteria.Add(Restrictions.Eq(syncTlaProp, false));
                    criteria = criteria.Add(Restrictions.Or(Restrictions.Eq(syncTlaProp, false), Restrictions.IsNull(syncTlaProp)));
                }

                if (queryParam.SyncTlaQuery == true)
                {
                    criteria = criteria.Add(Restrictions.Eq(syncTlaProp, true));
                }
                */

                if (queryParam.SyncTlaQuery == false)
                {
                    orderWaitPackListQuery = orderWaitPackListQuery.Where(e => e.FirstLogisticsMode.SyncTla == false || e.FirstLogisticsMode.SyncTla == null);
                }

                if (queryParam.SyncTlaQuery == true)
                {
                    orderWaitPackListQuery = orderWaitPackListQuery.Where(e => e.FirstLogisticsMode.SyncTla == true);
                }
            }

            if (queryParam.PackPrintQuery != null)
            {
                /*
                string packPrintTypeProp = $"fm.{nameof(LogisticsModeInfo.PackPrintType)}";

                if (queryParam.PackPrintQuery == false)
                {
                    criteria = criteria.Add(Restrictions.Eq(packPrintTypeProp, LogisticsOrderPackPrintType.None));
                }

                if (queryParam.PackPrintQuery == true)
                {
                    criteria = criteria.Add(Restrictions.Not(Restrictions.Eq(packPrintTypeProp, LogisticsOrderPackPrintType.None)));
                }
                */

                if (queryParam.PackPrintQuery == false)
                {
                    orderWaitPackListQuery = orderWaitPackListQuery.Where(e => e.FirstLogisticsMode.PackPrintType == LogisticsOrderPackPrintType.None);
                }

                if (queryParam.PackPrintQuery == true)
                {
                    orderWaitPackListQuery = orderWaitPackListQuery.Where(e => e.FirstLogisticsMode.PackPrintType != LogisticsOrderPackPrintType.None);
                }
            }

            if (queryParam.SubmitPackTimeBeginQuery != null)
            {
                /*
                // DateTime submitPackTimeBeginQuery = queryParam.SubmitPackTimeBeginQuery.Value.Date;
                DateTime submitPackTimeBeginQuery = queryParam.SubmitPackTimeBeginQuery.Value;
                criteria = criteria.Add(Restrictions.Ge($"order.{nameof(LogisticsOrderInfo.SubmitPackTime)}", submitPackTimeBeginQuery));
                */

                DateTime submitPackTimeBeginQuery = queryParam.SubmitPackTimeBeginQuery.Value;
                orderWaitPackListQuery = orderWaitPackListQuery.Where(e => e.SubmitPackTime >= submitPackTimeBeginQuery);
            }

            if (queryParam.SubmitPackTimeEndQuery != null)
            {
                /*
                // DateTime submitPackTimeEndQuery = queryParam.SubmitPackTimeEndQuery.Value.Date.AddDays(1);
                DateTime submitPackTimeEndQuery = queryParam.SubmitPackTimeEndQuery.Value;
                criteria = criteria.Add(Restrictions.Lt($"order.{nameof(LogisticsOrderInfo.SubmitPackTime)}", submitPackTimeEndQuery));
                */

                DateTime submitPackTimeEndQuery = queryParam.SubmitPackTimeEndQuery.Value;
                orderWaitPackListQuery = orderWaitPackListQuery.Where(e => e.SubmitPackTime < submitPackTimeEndQuery);
            }

            var orderIdList = orderWaitPackListQuery.Select(e => e.Id).ToArray();
            criteria = criteria.Add(Restrictions.In(claimOrderIdProp, orderIdList));
        }

        // 按界面分类过滤

        if (queryParam.WarehouseIdQuery != 0)
        {
            int warehouseIdQuery = queryParam.WarehouseIdQuery;
            criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressInfo.Warehouse)}.{nameof(ExpressInfo.Warehouse.Id)}", warehouseIdQuery));
        }

        IList<ExpressStatus> mayToOnShelfStatusList = ExpressStatus.GetMayToOnShelfList();

        // 待上架
        if (expressClaimCate == ExpressClaimCate.ToOnShelf)
        {
            /*
            IList<int> groundShelfIdList = session.Query<ShelfInfo>().Where(e => e.Cate == ShelfCate.Ground).Select(e => e.Id).ToList();
            criteria = criteria.Add(Restrictions.Not(Restrictions.In($"{shelfAlias}.{nameof(ShelfInfo.Id)}", groundShelfIdList.ToArray()))); // 待上架和待上架排班都不显示地面仓位
            */

            IList<int> groundStorageLocIdList = session.Query<StorageLocInfo>().Where(e => e.Shelf.Cate == ShelfCate.Ground).Select(e => e.Id).ToList(); // 找出所有地面仓位，应该不会很多
            criteria = criteria.Add(Restrictions.Not(Restrictions.In($"{nameof(ExpressInfo.StorageLoc)}.{nameof(StorageLocInfo.Id)}", groundStorageLocIdList.ToArray()))); // 待上架和待上架排班都不显示地面仓位                

            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.ShelfStatus == ExpressShelfStatus.ToOnShelf && e.AddTime >= EXPRESS_ON_SHELF_AFTER && e.ArchiveType == ExpressArchiveType.None));
            criteria = criteria.Add(Restrictions.In(nameof(ExpressInfo.Status), mayToOnShelfStatusList.ToArray()));
        }

        // 待上架排班
        if (expressClaimCate == ExpressClaimCate.ToOnShelfSchedule)
        {
            /*
            IList<int> groundShelfIdList = session.Query<ShelfInfo>().Where(e => e.Cate == ShelfCate.Ground).Select(e => e.Id).ToList();
            criteria = criteria.Add(Restrictions.Not(Restrictions.In($"{shelfAlias}.{nameof(ShelfInfo.Id)}", groundShelfIdList.ToArray()))); // 待上架和待上架排班都不显示地面仓位
            */

            IList<int> groundStorageLocIdList = session.Query<StorageLocInfo>().Where(e => e.Shelf.Cate == ShelfCate.Ground).Select(e => e.Id).ToList(); // 找出所有地面仓位，应该不会很多
            criteria = criteria.Add(Restrictions.Not(Restrictions.In($"{nameof(ExpressInfo.StorageLoc)}.{nameof(StorageLocInfo.Id)}", groundStorageLocIdList.ToArray()))); // 待上架和待上架排班都不显示地面仓位                  

            DetachedCriteria scheduleCriteria = DetachedCriteria.For<ExpressShelfScheduleInfo>();
            scheduleCriteria = scheduleCriteria.CreateAlias(nameof(ExpressShelfScheduleInfo.Claim), nameof(ExpressShelfScheduleInfo.Claim));
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.ScheduleType)}", ExpressShelfScheduleType.OnShelf));
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.Claim)}.{nameof(ExpressShelfScheduleInfo.Claim.ShelfStatus)}", ExpressShelfStatus.ToOnShelf));
            scheduleCriteria.SetProjection(Projections.Property($"{nameof(ExpressShelfScheduleInfo.Claim)}.{nameof(ExpressShelfScheduleInfo.Claim.Id)}"));

            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.ShelfStatus == ExpressShelfStatus.ToOnShelf && e.AddTime >= EXPRESS_ON_SHELF_AFTER && e.ArchiveType == ExpressArchiveType.None));
            criteria = criteria.Add(Restrictions.In(nameof(ExpressInfo.Status), mayToOnShelfStatusList.ToArray()));
            criteria = criteria.Add(Subqueries.PropertyNotIn($"{nameof(ExpressInfo.Id)}", scheduleCriteria));
        }

        // 已上架
        if (expressClaimCate == ExpressClaimCate.OnShelf)
        {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                e.AddTime >= EXPRESS_CHECKIN_RECENT_AGO && e.ShelfStatus == ExpressShelfStatus.OnShelf));
        }

        // 已认领
        if (expressClaimCate == ExpressClaimCate.Picked)
        {
            DetachedCriteria expectExpressCriteria = DetachedCriteria.For<ExpressExpectInfo>()
                .Add(Restrictions.Where<ExpressExpectInfo>(e => e.Status == ExpressExpectStatus.Create || e.Status == ExpressExpectStatus.Enter)) // create 状态放在这里只是为了容错
                .SetProjection(Projections.Property<ExpressExpectInfo>(e => e.TrackingNo));

            criteria = criteria.Add(Restrictions.And(
                    Restrictions.Where<ExpressInfo>(e => e.AddTime >= EXPRESS_CHECKIN_RECENT_AGO),
                    Restrictions.Or(
                        Restrictions.Where<ExpressInfo>(e => (e.Status == ExpressStatus.CheckIn || e.Status == ExpressStatus.WaitOrder)),
                        Subqueries.PropertyIn(nameof(ExpressInfo.TrackingNo), expectExpressCriteria)
                    )
                )
            );
        }

        // 未认领
        if (expressClaimCate == ExpressClaimCate.NotPicked)
        {
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
        if (expressClaimCate == ExpressClaimCate.Expect)
        {
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
        if (expressClaimCate == ExpressClaimCate.NotPickedLong)
        {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                e.AddTime >= ARCHIVE_DATE
                && e.AddTime < EXPRESS_CHECKIN_RECENT_AGO
                && e.Status == ExpressStatus.WaitPick
                && e.Dumped == false
                && e.ArchiveType == ExpressArchiveType.None)
            );
        }

        // 长期未出库
        if (expressClaimCate == ExpressClaimCate.NotOrderLong)
        {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                e.AddTime >= ARCHIVE_DATE
                && e.AddTime < EXPRESS_CHECKIN_RECENT_AGO
                && (e.Status == ExpressStatus.CheckIn || e.Status == ExpressStatus.WaitOrder)
                && e.Dumped == false
                && e.ArchiveType == ExpressArchiveType.None)
            );
        }

        // 长期未认领堆存排班
        if (expressClaimCate == ExpressClaimCate.NotPickedLongDumpSchedule)
        {
            DetachedCriteria scheduleCriteria = DetachedCriteria.For<ExpressDumpScheduleInfo>();
            scheduleCriteria = scheduleCriteria.CreateAlias(nameof(ExpressDumpScheduleInfo.Claim), nameof(ExpressDumpScheduleInfo.Claim));
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressDumpScheduleInfo.DumpType)}", ExpressDumpType.NoPickup));
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressDumpScheduleInfo.Claim)}.{nameof(ExpressDumpScheduleInfo.Claim.Dumped)}", false));
            scheduleCriteria.SetProjection(Projections.Property($"{nameof(ExpressDumpScheduleInfo.Claim)}.{nameof(ExpressDumpScheduleInfo.Claim.Id)}"));

            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                e.AddTime >= ARCHIVE_DATE
                && e.AddTime < EXPRESS_CHECKIN_RECENT_AGO
                && e.Status == ExpressStatus.WaitPick
                && e.Dumped == false
                && e.ArchiveType == ExpressArchiveType.None));

            criteria = criteria.Add(Subqueries.PropertyNotIn($"{nameof(ExpressInfo.Id)}", scheduleCriteria));
        }

        // 长期未出库堆存排班
        if (expressClaimCate == ExpressClaimCate.NotOrderLongDumpSchedule)
        {
            DetachedCriteria scheduleCriteria = DetachedCriteria.For<ExpressDumpScheduleInfo>();
            scheduleCriteria = scheduleCriteria.CreateAlias(nameof(ExpressDumpScheduleInfo.Claim), nameof(ExpressDumpScheduleInfo.Claim));
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressDumpScheduleInfo.DumpType)}", ExpressDumpType.NoProcess));
            scheduleCriteria.Add(Restrictions.Eq($"{nameof(ExpressDumpScheduleInfo.Claim)}.{nameof(ExpressDumpScheduleInfo.Claim.Dumped)}", false));
            scheduleCriteria.SetProjection(Projections.Property($"{nameof(ExpressDumpScheduleInfo.Claim)}.{nameof(ExpressDumpScheduleInfo.Claim.Id)}"));

            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                e.AddTime >= ARCHIVE_DATE
                && e.AddTime < EXPRESS_CHECKIN_RECENT_AGO
                && (e.Status == ExpressStatus.CheckIn || e.Status == ExpressStatus.WaitOrder)
                && e.Dumped == false
                && e.ArchiveType == ExpressArchiveType.None));

            criteria = criteria.Add(Subqueries.PropertyNotIn($"{nameof(ExpressInfo.Id)}", scheduleCriteria));
        }

        // 已堆存
        if (expressClaimCate == ExpressClaimCate.Dumped)
        {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Dumped == true));
        }

        // 历史归档
        if (expressClaimCate == ExpressClaimCate.Archive)
        {
            if (queryParam.ArchiveTypeQuery == null)
            {
                queryParam.ArchiveTypeQuery = ExpressArchiveType.None;
            }

            if (queryParam.ArchiveTypeQuery == ExpressArchiveType.None)
            {
                criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                    e.ArchiveType == ExpressArchiveType.ToOnShelf
                    || e.ArchiveType == ExpressArchiveType.ToOffShelf
                    || ((e.AddTime < ARCHIVE_DATE && e.Status == ExpressStatus.WaitPick && e.ArchiveType == ExpressArchiveType.None) || e.ArchiveType == ExpressArchiveType.NotPickedLong)
                    || ((e.AddTime < ARCHIVE_DATE && (e.Status == ExpressStatus.CheckIn || e.Status == ExpressStatus.WaitOrder) && e.ArchiveType == ExpressArchiveType.None) || e.ArchiveType == ExpressArchiveType.NotOrderLong)
                ));
            }

            if (queryParam.ArchiveTypeQuery == ExpressArchiveType.ToOnShelf)
            {
                criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                    e.ArchiveType == ExpressArchiveType.ToOnShelf
                ));
            }

            if (queryParam.ArchiveTypeQuery == ExpressArchiveType.ToOffShelf)
            {
                criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                    e.ArchiveType == ExpressArchiveType.ToOffShelf
                ));
            }

            if (queryParam.ArchiveTypeQuery == ExpressArchiveType.NotPickedLong)
            {
                criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                    ((e.AddTime < ARCHIVE_DATE && e.Status == ExpressStatus.WaitPick && e.ArchiveType == ExpressArchiveType.None) || e.ArchiveType == ExpressArchiveType.NotPickedLong)
                ));
            }

            if (queryParam.ArchiveTypeQuery == ExpressArchiveType.NotOrderLong)
            {
                criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e =>
                    ((e.AddTime < ARCHIVE_DATE && (e.Status == ExpressStatus.CheckIn || e.Status == ExpressStatus.WaitOrder) && e.ArchiveType == ExpressArchiveType.None) || e.ArchiveType == ExpressArchiveType.NotOrderLong)
                ));
            }
        }

        // 已下架
        if (expressClaimCate == ExpressClaimCate.OffShelf)
        {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.OffShelf));
        }

        // 打包中
        if (expressClaimCate == ExpressClaimCate.Packing)
        {
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.Packing));
        }

        string aoPinTrackingNoQuery = queryParam.AoPinTrackingNoQuery;
        if (string.IsNullOrWhiteSpace(aoPinTrackingNoQuery) == false)
        {
            aoPinTrackingNoQuery = aoPinTrackingNoQuery.Trim();
            // criteria = criteria.Add(Restrictions.On<ExpressInfo>(e => e.AoPinTrackingNo).IsInsensitiveLike(aoPinTrackingNoQuery, MatchMode.Anywhere));
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.AoPinTrackingNo == aoPinTrackingNoQuery));
        }

        string trackingNoQuery = queryParam.TrackingNoQuery;
        if (string.IsNullOrWhiteSpace(trackingNoQuery) == false)
        {
            trackingNoQuery = trackingNoQuery.Trim();
            // criteria = criteria.Add(Restrictions.On<ExpressInfo>(e => e.TrackingNo).IsInsensitiveLike(trackingNoQuery, MatchMode.Anywhere));
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.TrackingNo == trackingNoQuery));
        }

        string billCustomerCodeQuery = queryParam.BillCustomerCodeQuery;
        if (string.IsNullOrWhiteSpace(billCustomerCodeQuery) == false)
        {
            billCustomerCodeQuery = billCustomerCodeQuery.Trim();
            // criteria = criteria.Add(Restrictions.On<ExpressInfo>(e => e.BillCustomerCode).IsInsensitiveLike(billCustomerCodeQuery, MatchMode.Anywhere));
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.BillCustomerCode == billCustomerCodeQuery));
        }

        if (queryParam.HasBillCustomerIdQuery != null)
        {
            if (queryParam.HasBillCustomerIdQuery.Value == false) // 解析失败的
            {
                criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.BillCustomerId == 0));
            }
            else // 解析成功的
            {
                criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.BillCustomerId != 0));
            }
        }

        string customerCodeQuery = queryParam.CustomerCodeQuery;
        if (string.IsNullOrWhiteSpace(customerCodeQuery) == false)
        {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(customerCodeQuery);
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.CustomerId == customerIdQuery));
        }

        string orderCodeQuery = queryParam.OrderCodeQuery;
        if (string.IsNullOrWhiteSpace(orderCodeQuery) == false)
        {
            LogisticsOrderInfo order = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCodeQuery)
                .Take(1)
                .SingleOrDefault();

            int orderId = order?.Id ?? 0;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Order.Id == orderId));
        }

        string expressCompanyQuery = queryParam.ExpressCompanyQuery;
        if (string.IsNullOrWhiteSpace(expressCompanyQuery) == false)
        {
            expressCompanyQuery = expressCompanyQuery.Trim();
            // criteria = criteria.Add(Restrictions.On<ExpressInfo>(e => e.ExpressCompany).IsInsensitiveLike(expressCompanyQuery, MatchMode.Anywhere));
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.ExpressCompany == expressCompanyQuery));
        }

        if (queryParam.StatusQuery != null && queryParam.StatusQuery != ExpressStatus.None)
        {
            ExpressStatus statusQuery = queryParam.StatusQuery;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Status == statusQuery));
        }

        if (queryParam.ShelfStatusQuery != null && queryParam.ShelfStatusQuery != ExpressShelfStatus.None)
        {
            ExpressShelfStatus shelfStatusQuery = queryParam.ShelfStatusQuery;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.ShelfStatus == shelfStatusQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.GoodsNameQuery) == false)
        {
            string goodsNameQuery = queryParam.GoodsNameQuery.Trim();
            IList<int> goodsIdList = session.Query<LogisticsGoodsInfo>()
                .Where(e => e.Name.Contains(goodsNameQuery))
                .Select(e => e.Id)
                .ToList();

            IList<int> claimGoodsIdList = session.Query<LogisticsClaimGoodsEntryInfo>()
                .Where(e => goodsIdList.Contains(e.GoodsId))
                .Select(e => e.ClaimId)
                .Distinct()
                .ToList();

            criteria = criteria.Add(Restrictions.On<ExpressInfo>(e => e.Id).IsIn(claimGoodsIdList.ToArray()));
        }

        if (queryParam.AddDateBeginQuery != null)
        {
            DateTime addDateBeginQueryValue = queryParam.AddDateBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.AddTime >= addDateBeginQueryValue));
        }

        if (queryParam.AddDateEndQuery != null)
        {
            DateTime addDateEndQueryValue = queryParam.AddDateEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.AddTime < addDateEndQueryValue));
        }

        if (queryParam.AddTimeBeginQuery != null)
        {
            DateTime addTimeBeginQueryValue = queryParam.AddTimeBeginQuery.Value;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.AddTime >= addTimeBeginQueryValue));
        }

        if (queryParam.AddTimeEndQuery != null)
        {
            DateTime addTimeEndQueryValue = queryParam.AddTimeEndQuery.Value;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.AddTime <= addTimeEndQueryValue));
        }

        if (queryParam.OnShelfTimeBeginQuery != null)
        {
            DateTime OnShelfTimeBeginQueryValue = queryParam.OnShelfTimeBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.OnShelfTime >= OnShelfTimeBeginQueryValue));
        }

        if (queryParam.OnShelfTimeEndQuery != null)
        {
            DateTime OnShelfTimeEndQueryValue = queryParam.OnShelfTimeEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.OnShelfTime < OnShelfTimeEndQueryValue));
        }

        if (queryParam.PickTimeBeginQuery != null)
        {
            DateTime pickTimeBeginQueryValue = queryParam.PickTimeBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.ClaimTime >= pickTimeBeginQueryValue));
        }

        if (queryParam.PickTimeEndQuery != null)
        {
            DateTime pickTimeEndQueryValue = queryParam.PickTimeEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.ClaimTime < pickTimeEndQueryValue));
        }

        if (queryParam.DumpTimeBeginQuery != null)
        {
            DateTime dumpTimeBeginQueryValue = queryParam.DumpTimeBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.DumpTime >= dumpTimeBeginQueryValue));
        }

        if (queryParam.DumpTimeEndQuery != null)
        {
            DateTime dumpTimeEndQueryValue = queryParam.DumpTimeEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.DumpTime < dumpTimeEndQueryValue));
        }

        if (queryParam.ScheduleShelfIdListQuery != null && queryParam.ScheduleShelfIdListQuery.Count != 0)
        {
            /*
            string shelfIdProp = $"{shelfAlias}.{nameof(ExpressInfo.StorageLoc.Shelf.Id)}";
            criteria = criteria.Add(Restrictions.In(shelfIdProp, queryParam.ScheduleShelfIdListQuery.ToArray()));
            */

            var shelfIdListQuery = queryParam.ScheduleShelfIdListQuery;
            var shelfStorageLocIdList = session.Query<StorageLocInfo>().Where(e => shelfIdListQuery.Contains(e.Shelf.Id)).Select(e => e.Id).ToList();
            criteria = criteria.Add(Restrictions.In($"{nameof(ExpressInfo.StorageLoc)}.{nameof(StorageLocInfo.Id)}", shelfStorageLocIdList));
        }

        if (queryParam.ExpectedQuery != null)
        {
            bool expectedQueryValue = queryParam.ExpectedQuery.Value;


            if (expectedQueryValue == true)
            {
                DetachedCriteria expectExpressCriteria = DetachedCriteria.For<ExpressExpectInfo>()
                    .Add(Restrictions.Or(
                        Restrictions.Eq(nameof(ExpressExpectInfo.Status), ExpressExpectStatus.Create),
                        Restrictions.Eq(nameof(ExpressExpectInfo.Status), ExpressExpectStatus.Enter))
                    )
                    .SetProjection(Projections.Property<ExpressExpectInfo>(e => e.TrackingNo));

                criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.Status == ExpressStatus.WaitPick));

                criteria = criteria.Add(Subqueries.PropertyIn(nameof(ExpressInfo.TrackingNo), expectExpressCriteria));
            }
            else
            {
                DetachedCriteria expectExpressCriteria = DetachedCriteria.For<ExpressExpectInfo>()
                    .SetProjection(Projections.Property<ExpressExpectInfo>(e => e.TrackingNo));

                criteria = criteria.Add(Subqueries.PropertyNotIn(nameof(ExpressInfo.TrackingNo), expectExpressCriteria));
            }
        }

        if (queryParam.CheckinUserIdQuery != null)
        {
            int checkinUserIdQuery = queryParam.CheckinUserIdQuery.Value;
            criteria = criteria.Add(Restrictions.Where<ExpressInfo>(e => e.AddUser.Id == checkinUserIdQuery));
        }

        return criteria;
    }
}
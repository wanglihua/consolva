using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Domain.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressDumpScheduleService
{
    private ISession session;
    private ExpressSizeWeightService expressSizeWeightService;

    public ExpressDumpScheduleService(ISession session, ExpressSizeWeightService expressSizeWeightService)
    {
        this.session = session;
        this.expressSizeWeightService = expressSizeWeightService;
    }

    /// <summary>
    /// 分页获取列表数据
    /// </summary>
    /// <param name="dumpType"></param>
    /// <param name="queryParam"></param>
    /// <param name="listPager"></param>
    /// <returns></returns>
    public IList<ExpressDumpScheduleInfo> List(ExpressDumpType dumpType, ExpressListQueryParam queryParam, ListPager? listPager)
    {
        ICriteria criteria = GetListCriteria(dumpType, queryParam);

        if (listPager != null)
        {
            criteria = criteria.AddOrder(new Order(listPager.SortField, listPager.IsAscending))
                .AddOrder(Order.Desc(nameof(Entity.Id)))
                .SetFirstResult(listPager.PageIndex * listPager.PageSize)
                .SetMaxResults(listPager.PageSize);
        }

        IList<ExpressDumpScheduleInfo> scheduleList = criteria.List<ExpressDumpScheduleInfo>();

        IList<int> userIdList = scheduleList.Select(e => e.Express.AddUser.Id).Distinct().ToList();
        IList<int> customerIdList = scheduleList.Where(e => e.Express.Customer != null).Select(e => e.Express.Customer!.Id).Distinct().ToList();
        IList<int> orderIdList = scheduleList.Where(e => e.Express.Order != null).Select(e => e.Express.Order!.Id).Distinct().ToList();
        List<int> storageLocIdList = scheduleList.Where(e => e.Express.StorageLoc != null).Select(e => e.Express.StorageLoc.Id).Distinct().ToList();

        // 就当做是预加载了
        IList<UserInfo> userList = session.Query<UserInfo>().Where(e => e.LoginName != UserInfo.DevLoginName && userIdList.Contains(e.Id)).ToList();
        IList<CustomerInfo> customerList = session.Query<CustomerInfo>().Where(e => customerIdList.Contains(e.Id)).ToList();
        IList<LogisticsOrderInfo> orderList = session.Query<LogisticsOrderInfo>().Where(e => orderIdList.Contains(e.Id)).ToList();
        IList<StorageLocInfo> storageLocList = session.Query<StorageLocInfo>().Where(e => storageLocIdList.Contains(e.Id)).ToList();

        IList<string> trackingNoList = scheduleList.Select(e => e.Express.TrackingNo).ToList();
        IList<ExpressSizeWeightInfo> expressItemList = expressSizeWeightService.GetExpressSizeWeightList(trackingNoList);

        foreach (ExpressDumpScheduleInfo schedule in scheduleList)
        {
            /*
            UserInfo user = userList.Where(e => e.Id == schedule.Express.AddUserId).Take(1).SingleOrDefault();
            schedule.Express.AddUserNameCache = user?.UserName;

            CustomerInfo customer = customerList.Where(e => e.Id == schedule.Express.CustomerId).Take(1).SingleOrDefault();
            schedule.Express.CustomerNameCache = customer?.CustomerCode;

            LogisticsOrderInfo logisticsOrder = orderList.Where(e => e.Id == (schedule.Express.Order?.Id ?? 0)).Take(1).SingleOrDefault();
            schedule.Express.OrderCodeCache = logisticsOrder?.Code;

            StorageLocInfo storageLoc = storageLocList.Where(e => e.Id == (schedule.Express.StorageLoc?.Id ?? 0)).Take(1).SingleOrDefault();
            schedule.Express.StorageLocNameCache = storageLoc?.GetFullName();
            */

            // todo a 这个要启用起来
            schedule.Express.cache = expressItemList.Where(e => e.ExpressTrackingNo == schedule.Express.TrackingNo).ToList();
        }

        return scheduleList;
    }

    /// <summary>
    /// 不分页获取列表数据
    /// </summary>
    /// <param name="dumpType"></param>
    /// <param name="queryParam"></param>
    /// <returns></returns>
    public IList<ExpressDumpScheduleInfo> List(ExpressDumpType dumpType, ExpressListQueryParam queryParam)
    {
        return List(dumpType, queryParam, null);
    }

    /// <summary>
    /// 计数
    /// </summary>
    /// <param name="dumpType"></param>
    /// <param name="queryParam"></param>
    /// <returns></returns>
    public int Count(ExpressDumpType dumpType, ExpressListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(dumpType, queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        return totalCount;
    }

    /// <summary>
    /// 获取查询 criteria
    /// </summary>
    /// <param name="dumpType"></param>        
    /// <param name="queryParam"></param>
    /// <returns></returns>
    private ICriteria GetListCriteria(ExpressDumpType dumpType, ExpressListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<ExpressDumpScheduleInfo>();
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(ExpressDumpScheduleInfo.Express), "cg");

        string storageLoc = $"cg.{nameof(ExpressInfo.StorageLoc)}";
        criteria = criteria.CreateAlias(storageLoc, "sl");

        // 这个好像不起作用，生成的SQL，还是没有做这个join
        string shelf = $"sl.{nameof(StorageLocInfo.Shelf)}";
        criteria = criteria.CreateAlias(shelf, "shelf");

        criteria = criteria.CreateAlias(nameof(ExpressDumpScheduleInfo.User), "user");

        string claimWarehouse = $"cg.{nameof(ExpressInfo.Warehouse)}";
        criteria = criteria.CreateAlias(claimWarehouse, "w", JoinType.LeftOuterJoin);

        // 按排班类型过滤
        criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressDumpScheduleInfo.DumpType)}", dumpType));
        criteria = criteria.Add(Restrictions.Eq($"cg.{nameof(ExpressInfo.DumpStatus)}", ExpressDumpStatus.NotDump)); // 只显示未堆存的        

        // 不显示已打包和已退运的，这两个状态是绝对不会在货架上了
        var noDumpClaimStatusList = new ExpressStatus[] { ExpressStatus.Pack, ExpressStatus.Returned };
        criteria = criteria.Add(Restrictions.Not(Restrictions.In($"cg.{nameof(ExpressInfo.Status)}", noDumpClaimStatusList)));

        // 按查询条件过滤

        if (queryParam.WarehouseIdQuery != 0)
        {
            int warehouseIdQuery = queryParam.WarehouseIdQuery;
            criteria = criteria.Add(Restrictions.Eq($"w.{nameof(WarehouseInfo.Id)}", warehouseIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CheckinTrackingNoQuery) == false)
        {
            string checkinTrackingNoQuery = queryParam.CheckinTrackingNoQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"cg.{nameof(ExpressInfo.CheckinTrackingNo)}", checkinTrackingNoQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.TrackingNoQuery) == false)
        {
            string trackingNoQuery = queryParam.TrackingNoQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"cg.{nameof(ExpressInfo.TrackingNo)}", trackingNoQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.BillCustomerCodeQuery) == false)
        {
            string billCustomerCodeQuery = queryParam.BillCustomerCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"cg.{nameof(ExpressInfo.BillCustomerCode)}", billCustomerCodeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.ExpressCompanyQuery) == false)
        {
            string expressCompanyQuery = queryParam.ExpressCompanyQuery.Trim();
            criteria = criteria.Add(Restrictions.InsensitiveLike($"cg.{nameof(ExpressInfo.ExpressCompany)}", expressCompanyQuery, MatchMode.Anywhere));
        }

        if (queryParam.StatusQuery != null && queryParam.StatusQuery != ExpressStatus.None)
        {
            ExpressStatus statusQuery = queryParam.StatusQuery;
            criteria = criteria.Add(Restrictions.Eq($"cg.{nameof(ExpressInfo.Status)}", statusQuery));
        }

        if (queryParam.ShelfStatusQuery != null && queryParam.ShelfStatusQuery != ExpressShelfStatus.None)
        {
            ExpressShelfStatus shelfStatusQuery = queryParam.ShelfStatusQuery;
            criteria = criteria.Add(Restrictions.Eq($"cg.{nameof(ExpressInfo.ShelfStatus)}", shelfStatusQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.GoodsNameQuery) == false)
        {
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

            criteria = criteria.Add(Restrictions.In($"cg.{nameof(ExpressInfo.Id)}", expressIdList.ToArray()));
        }

        if (queryParam.ScheduleUserIdListQuery != null && queryParam.ScheduleUserIdListQuery.Count != 0)
        {
            // criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressDumpScheduleInfo.ScheduleType)}", queryParam.ScheduleType)); 这个前面已经有限制条件了
            criteria = criteria.Add(Restrictions.In($"user.{nameof(UserInfo.Id)}", queryParam.ScheduleUserIdListQuery.ToArray()));
        }

        if (queryParam.AddDateBeginQuery != null)
        {
            DateTime addTimeBeginQueryValue = queryParam.AddDateBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Ge($"cg.{nameof(ExpressInfo.AddTime)}", addTimeBeginQueryValue));
        }

        if (queryParam.AddDateEndQuery != null)
        {
            DateTime addTimeEndQueryValue = queryParam.AddDateEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Lt($"cg.{nameof(ExpressInfo.AddTime)}", addTimeEndQueryValue));
        }

        if (queryParam.OnShelfTimeBeginQuery != null)
        {
            DateTime onShelfTimeBeginQueryValue = queryParam.OnShelfTimeBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Ge($"cg.{nameof(ExpressInfo.OnShelfTime)}", onShelfTimeBeginQueryValue));
        }

        if (queryParam.OnShelfTimeEndQuery != null)
        {
            DateTime onShelfTimeEndQueryValue = queryParam.OnShelfTimeEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Lt($"cg.{nameof(ExpressInfo.OnShelfTime)}", onShelfTimeEndQueryValue));
        }

        // 看上下文，如果是上架的，就是上架排班时间；如果是下架的，就是下架排班时间
        if (queryParam.ScheduleTimeBeginQuery != null)
        {
            DateTime scheduleTimeBeginQueryValue = queryParam.ScheduleTimeBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Ge($"{nameof(ExpressDumpScheduleInfo.CreateTime)}", scheduleTimeBeginQueryValue));
        }

        if (queryParam.ScheduleTimeEndQuery != null)
        {
            DateTime scheduleTimeEndQueryValue = queryParam.ScheduleTimeEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Lt($"{nameof(ExpressDumpScheduleInfo.CreateTime)}", scheduleTimeEndQueryValue));
        }

        if (queryParam.ClaimTimeBeginQuery != null)
        {
            DateTime pickTimeBeginQueryValue = queryParam.ClaimTimeBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Ge($"cg.{nameof(ExpressInfo.ClaimTime)}", pickTimeBeginQueryValue));
        }

        if (queryParam.ClaimTimeEndQuery != null)
        {
            DateTime pickTimeEndQueryValue = queryParam.ClaimTimeEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Lt($"cg.{nameof(ExpressInfo.ClaimTime)}", pickTimeEndQueryValue));
        }

        if (queryParam.ScheduleShelfIdListQuery != null && queryParam.ScheduleShelfIdListQuery.Count != 0)
        {
            string shelfIdProp = $"shelf.{nameof(ShelfInfo.Id)}";
            criteria = criteria.Add(Restrictions.In(shelfIdProp, queryParam.ScheduleShelfIdListQuery.ToArray()));
        }

        return criteria;
    }
}
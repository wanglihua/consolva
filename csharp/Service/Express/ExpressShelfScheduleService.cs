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

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressShelfScheduleService
{
    private ISession session;
    private ExpressSizeWeightService expressSizeWeightService;

    public ExpressShelfScheduleService(ISession session, ExpressSizeWeightService expressSizeWeightService)
    {
        this.session = session;
        this.expressSizeWeightService = expressSizeWeightService;
    }

    /// <summary>
    /// 分页获取列表数据
    /// </summary>
    /// <param name="scheduleType"></param>
    /// <param name="queryParam"></param>
    /// <param name="listPager"></param>
    /// <returns></returns>
    public IList<ExpressShelfScheduleInfo> List(ExpressShelfScheduleType scheduleType, ExpressClaimListQueryParam queryParam, ListPager? listPager)
    {
        ICriteria criteria = GetListCriteria(scheduleType, queryParam);

        if (listPager != null)
        {
            criteria = criteria.AddOrder(new Order(listPager.SortField, listPager.IsAscending))
                .AddOrder(Order.Desc(nameof(Entity.Id)))
                .SetFirstResult(listPager.PageIndex * listPager.PageSize)
                .SetMaxResults(listPager.PageSize);
        }

        IList<ExpressShelfScheduleInfo> scheduleList = criteria.List<ExpressShelfScheduleInfo>();

        IList<int> userIdList = scheduleList.Select(e => e.Express.AddUser.Id).Distinct().ToList();
        IList<int> customerIdList = scheduleList.Where(e => e.Express.Customer != null).Select(e => e.Express.Customer!.Id).Distinct().ToList();
        IList<int> orderIdList = scheduleList.Where(e => e.Express.Order != null).Select(e => e.Express.Order!.Id).Distinct().ToList();
        List<int> storageLocIdList = scheduleList.Where(e => e.Express.StorageLoc != null).Select(e => e.Express.StorageLoc.Id).Distinct().ToList();

        // 就当时预加载了
        IList<UserInfo> userList = session.Query<UserInfo>().Where(e => e.LoginName != UserInfo.DevLoginName && userIdList.Contains(e.Id)).ToList();
        IList<CustomerInfo> customerList = session.Query<CustomerInfo>().Where(e => customerIdList.Contains(e.Id)).ToList();
        IList<LogisticsOrderInfo> orderList = session.Query<LogisticsOrderInfo>().Where(e => orderIdList.Contains(e.Id)).ToList();
        IList<StorageLocInfo> storageLocList = session.Query<StorageLocInfo>().Where(e => storageLocIdList.Contains(e.Id)).ToList();

        IList<string> trackingNoList = scheduleList.Select(e => e.Express.TrackingNo).ToList();
        IList<ExpressSizeWeightInfo> expressItemList = expressSizeWeightService.GetExpressSizeWeightList(trackingNoList);

        foreach (ExpressShelfScheduleInfo schedule in scheduleList)
        {
            // UserInfo user = userList.Where(e => e.Id == schedule.Express.AddUserId).Take(1).SingleOrDefault();
            // schedule.Express.AddUserNameCache = user?.UserName;
            //
            // CustomerInfo customer = customerList.Where(e => e.Id == schedule.Express.CustomerId).Take(1).SingleOrDefault();
            // schedule.Express.CustomerNameCache = customer?.CustomerCode;
            //
            // LogisticsOrderInfo logisticsOrder = orderList.Where(e => e.Id == (schedule.Express.Order?.Id ?? 0)).Take(1).SingleOrDefault();
            // schedule.Express.OrderCodeCache = logisticsOrder?.Code;
            //
            // StorageLocInfo storageLoc = storageLocList.Where(e => e.Id == (schedule.Express.StorageLoc?.Id ?? 0)).Take(1).SingleOrDefault();
            // schedule.Express.StorageLocNameCache = storageLoc?.GetFullName();

            // todo a 启用这个字段
            // schedule.Express.ExpressItemListCache = expressItemList.Where(e => e.ExpressTrackingNo == schedule.Express.TrackingNo).ToList();
        }

        return scheduleList;
    }

    /// <summary>
    /// 不分页获取列表数据
    /// </summary>
    /// <param name="scheduleType"></param>
    /// <param name="queryParam"></param>
    /// <returns></returns>
    public IList<ExpressShelfScheduleInfo> List(ExpressShelfScheduleType scheduleType, ExpressClaimListQueryParam queryParam)
    {
        return List(scheduleType, queryParam, null);
    }

    /// <summary>
    /// 计数
    /// </summary>
    /// <param name="scheduleType"></param>
    /// <param name="queryParam"></param>
    /// <returns></returns>
    public int Count(ExpressShelfScheduleType scheduleType, ExpressClaimListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(scheduleType, queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        return totalCount;
    }

    /// <summary>
    /// 获取查询 criteria
    /// </summary>
    /// <param name="scheduleType"></param>        
    /// <param name="queryParam"></param>
    /// <returns></returns>
    private ICriteria GetListCriteria(ExpressShelfScheduleType scheduleType, ExpressClaimListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<ExpressShelfScheduleInfo>();
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(ExpressShelfScheduleInfo.Express), "express");

        string storageLoc = $"express.{nameof(ExpressInfo.StorageLoc)}";
        criteria = criteria.CreateAlias(storageLoc, "sl");

        // 这个好像不起作用，生成的SQL，还是没有做这个join
        string shelf = $"sl.{nameof(StorageLocInfo.Shelf)}";
        criteria = criteria.CreateAlias(shelf, "shelf");

        criteria = criteria.CreateAlias(nameof(ExpressShelfScheduleInfo.User), "user");

        string claimWarehouse = $"express.{nameof(ExpressInfo.Warehouse)}";
        criteria = criteria.CreateAlias(claimWarehouse, "w", JoinType.InnerJoin);

        string orderFirstMode = $"express.{nameof(ExpressInfo.FirstMode)}";
        criteria = criteria.CreateAlias(orderFirstMode, "fm", JoinType.InnerJoin); // 使用 inner join，这时肯定已经有头程物流了

        // 按排班类型过滤
        criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.ScheduleType)}", scheduleType));

        // 按查询条件过滤

        if (queryParam.WarehouseIdQuery != 0)
        {
            int warehouseIdQuery = queryParam.WarehouseIdQuery;
            criteria = criteria.Add(Restrictions.Eq($"w.{nameof(WarehouseInfo.Id)}", warehouseIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.AoPinTrackingNoQuery) == false)
        {
            string checkinTrackingNoQuery = queryParam.AoPinTrackingNoQuery.Trim();
            criteria = criteria.Add(Restrictions.InsensitiveLike($"express.{nameof(ExpressInfo.CheckinTrackingNo)}", checkinTrackingNoQuery, MatchMode.Anywhere));
        }
        
        if (string.IsNullOrWhiteSpace(queryParam.TrackingNoQuery) == false)
        {
            string trackingNoQuery = queryParam.TrackingNoQuery.Trim();
            criteria = criteria.Add(Restrictions.InsensitiveLike($"express.{nameof(ExpressInfo.TrackingNo)}", trackingNoQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.BillCustomerCodeQuery) == false)
        {
            string billCustomerCodeQuery = queryParam.BillCustomerCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.InsensitiveLike($"express.{nameof(ExpressInfo.BillCustomerCode)}", billCustomerCodeQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.ExpressCompanyQuery) == false)
        {
            string expressCompanyQuery = queryParam.ExpressCompanyQuery.Trim();
            criteria = criteria.Add(Restrictions.InsensitiveLike($"express.{nameof(ExpressInfo.ExpressCompany)}", expressCompanyQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"order.{nameof(LogisticsOrderInfo.Code)}", orderCodeQuery));
        }

        if (scheduleType == ExpressShelfScheduleType.OnShelf)
        {
            // criteria = criteria.Add(Restrictions.Eq($"express.{nameof(ExpressInfo.Status)}", statusQuery));
            criteria = criteria.Add(Restrictions.Eq($"express.{nameof(ExpressInfo.ShelfStatus)}", ExpressShelfStatus.ToOnShelf)); // 只显示待上架的
        }

        if (scheduleType == ExpressShelfScheduleType.OffShelf)
        {
            criteria = criteria.Add(Restrictions.Eq($"express.{nameof(ExpressInfo.Status)}", ExpressStatus.WaitPack)); // 只显示待下架，就是待打包的
            // criteria = criteria.Add(Restrictions.Eq($"express.{nameof(ExpressInfo.ShelfStatus)}", ExpressShelfStatus.));
        }

        if (string.IsNullOrWhiteSpace(queryParam.GoodsNameQuery) == false)
        {
            string goodsNameQuery = queryParam.GoodsNameQuery.Trim();
            IList<int> goodsIdList = session.Query<LogisticsGoodsInfo>()
                .Where(e => e.Name.Contains(goodsNameQuery))
                .Select(e => e.Id)
                .ToList();

            IList<int> claimGoodsIdList = session.Query<ExpressGoodsInfo>()
                .Where(e => goodsIdList.Contains(e.Goods.Id))
                .Select(e => e.Express.Id)
                .Distinct()
                .ToList();

            criteria = criteria.Add(Restrictions.In($"express.{nameof(ExpressInfo.Id)}", claimGoodsIdList.ToArray()));
        }

        if (queryParam.ScheduleUserIdListQuery != null && queryParam.ScheduleUserIdListQuery.Count != 0)
        {
            // criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.ScheduleType)}", queryParam.ScheduleType)); 这个前面已经有限制条件了
            criteria = criteria.Add(Restrictions.In($"user.{nameof(UserInfo.Id)}", queryParam.ScheduleUserIdListQuery.ToArray()));
        }

        if (queryParam.AddDateBeginQuery != null)
        {
            DateTime addTimeBeginQueryValue = queryParam.AddDateBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Ge($"express.{nameof(ExpressInfo.AddTime)}", addTimeBeginQueryValue));
        }

        if (queryParam.AddDateEndQuery != null)
        {
            DateTime addTimeEndQueryValue = queryParam.AddDateEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Lt($"express.{nameof(ExpressInfo.AddTime)}", addTimeEndQueryValue));
        }

        if (queryParam.OnShelfTimeBeginQuery != null)
        {
            DateTime onShelfTimeBeginQueryValue = queryParam.OnShelfTimeBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Ge($"express.{nameof(ExpressInfo.OnShelfTime)}", onShelfTimeBeginQueryValue));
        }

        if (queryParam.OnShelfTimeEndQuery != null)
        {
            DateTime onShelfTimeEndQueryValue = queryParam.OnShelfTimeEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Lt($"express.{nameof(ExpressInfo.OnShelfTime)}", onShelfTimeEndQueryValue));
        }

        // 看上下文，如果是上架的，就是上架排班时间；如果是下架的，就是下架排班时间
        if (queryParam.ScheduleTimeBeginQuery != null)
        {
            DateTime scheduleTimeBeginQueryValue = queryParam.ScheduleTimeBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Ge($"{nameof(ExpressShelfScheduleInfo.CreateTime)}", scheduleTimeBeginQueryValue));
        }

        if (queryParam.ScheduleTimeEndQuery != null)
        {
            DateTime scheduleTimeEndQueryValue = queryParam.ScheduleTimeEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Lt($"{nameof(ExpressShelfScheduleInfo.CreateTime)}", scheduleTimeEndQueryValue));
        }

        if (queryParam.ClaimTimeBeginQuery != null)
        {
            DateTime pickTimeBeginQueryValue = queryParam.ClaimTimeBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Ge($"express.{nameof(ExpressInfo.ClaimTime)}", pickTimeBeginQueryValue));
        }

        if (queryParam.ClaimTimeEndQuery != null)
        {
            DateTime pickTimeEndQueryValue = queryParam.ClaimTimeEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Lt($"express.{nameof(ExpressInfo.ClaimTime)}", pickTimeEndQueryValue));
        }

        if (queryParam.ScheduleShelfIdListQuery != null && queryParam.ScheduleShelfIdListQuery.Count != 0)
        {
            string shelfIdProp = $"shelf.{nameof(ShelfInfo.Id)}";
            criteria = criteria.Add(Restrictions.In(shelfIdProp, queryParam.ScheduleShelfIdListQuery.ToArray()));
        }

        if (queryParam.FirstModeIdQuery != 0)
        {
            int firstModeIdQuery = queryParam.FirstModeIdQuery;
            criteria = criteria.Add(Restrictions.Eq($"express.{nameof(ExpressInfo.FirstMode)}.{nameof(LogisticsModeInfo.Id)}", firstModeIdQuery));
        }

        return criteria;
    }
}
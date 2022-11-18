using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Domain.Customer;
using Domain.Logistics;
using Domain.LogisticsOrder;
using Domain.Pass;
using Domain.Warehouse;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Service.LogisticsOrder;

namespace shunda.Service.Express;

namespace Service.Express
{
    [Service(ServiceType.Scoped)]
    public class ExpressShelfScheduleService
    {
        private ISession session = null;
        private ExpressSizeWeightService expressSizeWeightService = null;

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
        public IList<ExpressShelfScheduleInfo> List(ExpressShelfScheduleType scheduleType, ExpressClaimListQueryParam queryParam, ListPager listPager)
        {
            ICriteria criteria = GetListCriteria(scheduleType, queryParam);

            if (listPager != null)
            {
                criteria = criteria.AddOrder(new LogisticsOrder(listPager.SortField, listPager.IsAscending))
                    .AddOrder(LogisticsOrder.Desc(nameof(Model.Id)))
                    .SetFirstResult(listPager.PageIndex * listPager.PageSize)
                    .SetMaxResults(listPager.PageSize);
            }

            IList<ExpressShelfScheduleInfo> scheduleList = criteria.List<ExpressShelfScheduleInfo>();

            IList<int> userIdList = scheduleList.Select(e => e.Claim.AddUserId).Distinct().ToList();
            IList<int> customerIdList = scheduleList.Where(e => e.Claim.CustomerId.HasValue).Select(e => e.Claim.CustomerId.Value).Distinct().ToList();
            IList<int> orderIdList = scheduleList.Where(e => e.Claim.Order != null).Select(e => e.Claim.Order.Id).Distinct().ToList();
            List<int> storageLocIdList = scheduleList.Where(e => e.Claim.StorageLoc != null).Select(e => e.Claim.StorageLoc.Id).Distinct().ToList();

            IList<UserInfo> userList = session.Query<UserInfo>().Where(e => e.LoginName != UserInfo.DevLoginName && userIdList.Contains(e.Id)).ToList();
            IList<CustomerInfo> customerList = session.Query<CustomerInfo>().Where(e => customerIdList.Contains(e.Id)).ToList();
            IList<LogisticsOrderInfo> orderList = session.Query<LogisticsOrderInfo>().Where(e => orderIdList.Contains(e.Id)).ToList();
            IList<StorageLocInfo> storageLocList = session.Query<StorageLocInfo>().Where(e => storageLocIdList.Contains(e.Id)).ToList();

            IList<string> trackingNoList = scheduleList.Select(e => e.Claim.TrackingNo).ToList();
            IList<LogisticsExpressItem> expressItemList = expressSizeWeightService.GetExpressSizeWeightList(trackingNoList);

            foreach (ExpressShelfScheduleInfo schedule in scheduleList)
            {
                UserInfo user = userList.Where(e => e.Id == schedule.Claim.AddUserId).Take(1).SingleOrDefault();
                schedule.Claim.AddUserNameCache = user?.UserName;

                CustomerInfo customer = customerList.Where(e => e.Id == schedule.Claim.CustomerId).Take(1).SingleOrDefault();
                schedule.Claim.CustomerNameCache = customer?.CustomerCode;

                LogisticsOrderInfo logisticsOrder = orderList.Where(e => e.Id == (schedule.Claim.Order?.Id ?? 0)).Take(1).SingleOrDefault();
                schedule.Claim.OrderCodeCache = logisticsOrder?.Code;

                StorageLocInfo storageLoc = storageLocList.Where(e => e.Id == (schedule.Claim.StorageLoc?.Id ?? 0)).Take(1).SingleOrDefault();
                schedule.Claim.StorageLocNameCache = storageLoc?.GetFullName();

                schedule.Claim.ExpressItemListCache = expressItemList.Where(e => e.ExpressTrackingNo == schedule.Claim.TrackingNo).ToList();
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

            criteria = criteria.CreateAlias(nameof(ExpressShelfScheduleInfo.Claim), "cg");

            string storageLoc = $"cg.{nameof(LogisticsClaimGoodsInfo.StorageLoc)}";
            criteria = criteria.CreateAlias(storageLoc, "sl");

            // 这个好像不起作用，生成的SQL，还是没有做这个join
            string shelf = $"sl.{nameof(StorageLocInfo.Shelf)}";
            criteria = criteria.CreateAlias(shelf, "shelf");

            criteria = criteria.CreateAlias(nameof(ExpressShelfScheduleInfo.User), "user");

            string claimWarehouse = $"cg.{nameof(LogisticsClaimGoodsInfo.Warehouse)}";
            criteria = criteria.CreateAlias(claimWarehouse, "w", JoinType.LeftOuterJoin);

            string claimOrder = $"cg.{nameof(LogisticsClaimGoodsInfo.Order)}";
            criteria = criteria.CreateAlias(claimOrder, "order", JoinType.LeftOuterJoin);

            string orderWarehouse = $"order.{nameof(LogisticsOrderInfo.TargetWarehouse)}";
            criteria = criteria.CreateAlias(orderWarehouse, "tw", JoinType.LeftOuterJoin);

            string orderFirstMode = $"order.{nameof(LogisticsOrderInfo.FirstLogisticsMode)}";
            criteria = criteria.CreateAlias(orderFirstMode, "fm", JoinType.LeftOuterJoin);

            // 按排班类型过滤
            criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.ScheduleType)}", scheduleType));

            // 按查询条件过滤

            if (queryParam.WarehouseIdQuery != 0)
            {
                int warehouseIdQuery = queryParam.WarehouseIdQuery;
                criteria = criteria.Add(Restrictions.Eq($"w.{nameof(WarehouseInfo.Id)}", warehouseIdQuery));
            }

            string aoPinTrackingNoQuery = queryParam.AoPinTrackingNoQuery;
            if (string.IsNullOrWhiteSpace(aoPinTrackingNoQuery) == false)
            {
                aoPinTrackingNoQuery = aoPinTrackingNoQuery.Trim();
                criteria = criteria.Add(Restrictions.InsensitiveLike($"cg.{nameof(LogisticsClaimGoodsInfo.AoPinTrackingNo)}", aoPinTrackingNoQuery, MatchMode.Anywhere));
            }

            string trackingNoQuery = queryParam.TrackingNoQuery;
            if (string.IsNullOrWhiteSpace(trackingNoQuery) == false)
            {
                trackingNoQuery = trackingNoQuery.Trim();
                criteria = criteria.Add(Restrictions.InsensitiveLike($"cg.{nameof(LogisticsClaimGoodsInfo.TrackingNo)}", trackingNoQuery, MatchMode.Anywhere));
            }

            string customerCodeQuery = queryParam.BillCustomerCodeQuery;
            if (string.IsNullOrWhiteSpace(customerCodeQuery) == false)
            {
                customerCodeQuery = customerCodeQuery.Trim();
                criteria = criteria.Add(Restrictions.InsensitiveLike($"cg.{nameof(LogisticsClaimGoodsInfo.BillCustomerCode)}", customerCodeQuery, MatchMode.Anywhere));
            }

            string expressCompanyQuery = queryParam.ExpressCompanyQuery;
            if (string.IsNullOrWhiteSpace(expressCompanyQuery) == false)
            {
                expressCompanyQuery = expressCompanyQuery.Trim();
                criteria = criteria.Add(Restrictions.InsensitiveLike($"cg.{nameof(LogisticsClaimGoodsInfo.ExpressCompany)}", expressCompanyQuery, MatchMode.Anywhere));
            }

            string orderCodeQuery = queryParam.OrderCodeQuery;
            if (string.IsNullOrWhiteSpace(orderCodeQuery) == false)
            {
                orderCodeQuery = orderCodeQuery.Trim();
                criteria = criteria.Add(Restrictions.Eq($"order.{nameof(LogisticsOrderInfo.Code)}", orderCodeQuery));
            }

            if (scheduleType == ExpressShelfScheduleType.OnShelf)
            {
                // criteria = criteria.Add(Restrictions.Eq($"cg.{nameof(LogisticsClaimGoodsInfo.Status)}", statusQuery));
                criteria = criteria.Add(Restrictions.Eq($"cg.{nameof(LogisticsClaimGoodsInfo.ShelfStatus)}", ExpressShelfStatus.ToOnShelf)); // 只显示待上架的
            }

            if (scheduleType == ExpressShelfScheduleType.OffShelf)
            {
                criteria = criteria.Add(Restrictions.Eq($"cg.{nameof(LogisticsClaimGoodsInfo.Status)}", LogisticsClaimGoodsStatus.WaitPack)); // 只显示待下架，就是待打包的
                // criteria = criteria.Add(Restrictions.Eq($"cg.{nameof(LogisticsClaimGoodsInfo.ShelfStatus)}", ExpressShelfStatus.));
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

                criteria = criteria.Add(Restrictions.In($"cg.{nameof(LogisticsClaimGoodsInfo.Id)}", claimGoodsIdList.ToArray()));
            }

            if (queryParam.ScheduleUserIdListQuery != null && queryParam.ScheduleUserIdListQuery.Count != 0)
            {
                // criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressShelfScheduleInfo.ScheduleType)}", queryParam.ScheduleType)); 这个前面已经有限制条件了
                criteria = criteria.Add(Restrictions.In($"user.{nameof(UserInfo.Id)}", queryParam.ScheduleUserIdListQuery.ToArray()));
            }

            if (queryParam.AddDateBeginQuery != null)
            {
                DateTime addTimeBeginQueryValue = queryParam.AddDateBeginQuery.Value.Date;
                criteria = criteria.Add(Restrictions.Ge($"cg.{nameof(LogisticsClaimGoodsInfo.AddTime)}", addTimeBeginQueryValue));
            }

            if (queryParam.AddDateEndQuery != null)
            {
                DateTime addTimeEndQueryValue = queryParam.AddDateEndQuery.Value.Date.AddDays(1);
                criteria = criteria.Add(Restrictions.Lt($"cg.{nameof(LogisticsClaimGoodsInfo.AddTime)}", addTimeEndQueryValue));
            }

            if (queryParam.OnShelfTimeBeginQuery != null)
            {
                DateTime onShelfTimeBeginQueryValue = queryParam.OnShelfTimeBeginQuery.Value.Date;
                criteria = criteria.Add(Restrictions.Ge($"cg.{nameof(LogisticsClaimGoodsInfo.OnShelfTime)}", onShelfTimeBeginQueryValue));
            }

            if (queryParam.OnShelfTimeEndQuery != null)
            {
                DateTime onShelfTimeEndQueryValue = queryParam.OnShelfTimeEndQuery.Value.Date.AddDays(1);
                criteria = criteria.Add(Restrictions.Lt($"cg.{nameof(LogisticsClaimGoodsInfo.OnShelfTime)}", onShelfTimeEndQueryValue));
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

            if (queryParam.PickTimeBeginQuery != null)
            {
                DateTime pickTimeBeginQueryValue = queryParam.PickTimeBeginQuery.Value.Date;
                criteria = criteria.Add(Restrictions.Ge($"cg.{nameof(LogisticsClaimGoodsInfo.PickTime)}", pickTimeBeginQueryValue));
            }

            if (queryParam.PickTimeEndQuery != null)
            {
                DateTime pickTimeEndQueryValue = queryParam.PickTimeEndQuery.Value.Date.AddDays(1);
                criteria = criteria.Add(Restrictions.Lt($"cg.{nameof(LogisticsClaimGoodsInfo.PickTime)}", pickTimeEndQueryValue));
            }

            if (queryParam.ScheduleShelfIdListQuery != null && queryParam.ScheduleShelfIdListQuery.Count != 0)
            {
                string shelfIdProp = $"shelf.{nameof(ShelfInfo.Id)}";
                criteria = criteria.Add(Restrictions.In(shelfIdProp, queryParam.ScheduleShelfIdListQuery.ToArray()));
            }

            if (queryParam.OrderTargetWarehouseIdQuery != 0)
            {
                int orderTargetWarehouseIdQuery = queryParam.OrderTargetWarehouseIdQuery;
                criteria = criteria.Add(Restrictions.Eq($"order.{nameof(LogisticsOrderInfo.TargetWarehouse)}.{nameof(WarehouseInfo.Id)}", orderTargetWarehouseIdQuery));
            }

            if (queryParam.FirstModeIdQuery != 0)
            {
                int firstModeIdQuery = queryParam.FirstModeIdQuery;
                criteria = criteria.Add(Restrictions.Eq($"order.{nameof(LogisticsOrderInfo.FirstLogisticsMode)}.{nameof(LogisticsModeInfo.Id)}", firstModeIdQuery));
            }

            return criteria;
        }
    }
}
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
    public class ExpressDumpScheduleService
    {
        private ISession session = null;
        private ExpressSizeWeightService expressSizeWeightService = null;

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
        public IList<ExpressDumpScheduleInfo> List(ExpressDumpType dumpType, ExpressClaimListQueryParam queryParam, ListPager listPager)
        {
            ICriteria criteria = GetListCriteria(dumpType, queryParam);

            if (listPager != null)
            {
                criteria = criteria.AddOrder(new LogisticsOrder(listPager.SortField, listPager.IsAscending))
                    .AddOrder(LogisticsOrder.Desc(nameof(Model.Id)))
                    .SetFirstResult(listPager.PageIndex * listPager.PageSize)
                    .SetMaxResults(listPager.PageSize);
            }

            IList<ExpressDumpScheduleInfo> scheduleList = criteria.List<ExpressDumpScheduleInfo>();

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

            foreach (ExpressDumpScheduleInfo schedule in scheduleList)
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
        /// <param name="dumpType"></param>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        public IList<ExpressDumpScheduleInfo> List(ExpressDumpType dumpType, ExpressClaimListQueryParam queryParam)
        {
            return List(dumpType, queryParam, null);
        }

        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="dumpType"></param>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        public int Count(ExpressDumpType dumpType, ExpressClaimListQueryParam queryParam)
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
        private ICriteria GetListCriteria(ExpressDumpType dumpType, ExpressClaimListQueryParam queryParam)
        {
            ICriteria criteria = session.CreateCriteria<ExpressDumpScheduleInfo>();
            criteria = criteria.SetReadOnly(true);

            criteria = criteria.CreateAlias(nameof(ExpressDumpScheduleInfo.Claim), "cg");

            string storageLoc = $"cg.{nameof(LogisticsClaimGoodsInfo.StorageLoc)}";
            criteria = criteria.CreateAlias(storageLoc, "sl");

            // 这个好像不起作用，生成的SQL，还是没有做这个join
            string shelf = $"sl.{nameof(StorageLocInfo.Shelf)}";
            criteria = criteria.CreateAlias(shelf, "shelf");

            criteria = criteria.CreateAlias(nameof(ExpressDumpScheduleInfo.User), "user");

            string claimWarehouse = $"cg.{nameof(LogisticsClaimGoodsInfo.Warehouse)}";
            criteria = criteria.CreateAlias(claimWarehouse, "w", JoinType.LeftOuterJoin);

            // 按排班类型过滤
            criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressDumpScheduleInfo.DumpType)}", dumpType));
            criteria = criteria.Add(Restrictions.Eq($"cg.{nameof(LogisticsClaimGoodsInfo.Dumped)}", false)); // 只显示未堆存的        

            // 不显示已打包和已退运的，这两个状态是绝对不会在货架上了
            var noDumpClaimStatusList = new LogisticsClaimGoodsStatus[] {LogisticsClaimGoodsStatus.Pack, LogisticsClaimGoodsStatus.Returned};
            criteria = criteria.Add(Restrictions.Not(Restrictions.In($"cg.{nameof(LogisticsClaimGoodsInfo.Status)}", noDumpClaimStatusList)));

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
                criteria = criteria.Add(Restrictions.Eq($"cg.{nameof(LogisticsClaimGoodsInfo.AoPinTrackingNo)}", aoPinTrackingNoQuery));
            }

            string trackingNoQuery = queryParam.TrackingNoQuery;
            if (string.IsNullOrWhiteSpace(trackingNoQuery) == false)
            {
                trackingNoQuery = trackingNoQuery.Trim();
                criteria = criteria.Add(Restrictions.Eq($"cg.{nameof(LogisticsClaimGoodsInfo.TrackingNo)}", trackingNoQuery));
            }

            string customerCodeQuery = queryParam.BillCustomerCodeQuery;
            if (string.IsNullOrWhiteSpace(customerCodeQuery) == false)
            {
                customerCodeQuery = customerCodeQuery.Trim();
                criteria = criteria.Add(Restrictions.Eq($"cg.{nameof(LogisticsClaimGoodsInfo.BillCustomerCode)}", customerCodeQuery));
            }

            string expressCompanyQuery = queryParam.ExpressCompanyQuery;
            if (string.IsNullOrWhiteSpace(expressCompanyQuery) == false)
            {
                expressCompanyQuery = expressCompanyQuery.Trim();
                criteria = criteria.Add(Restrictions.InsensitiveLike($"cg.{nameof(LogisticsClaimGoodsInfo.ExpressCompany)}", expressCompanyQuery, MatchMode.Anywhere));
            }

            if (queryParam.StatusQuery != null && queryParam.StatusQuery != LogisticsClaimGoodsStatus.None)
            {
                LogisticsClaimGoodsStatus statusQuery = queryParam.StatusQuery;
                criteria = criteria.Add(Restrictions.Eq($"cg.{nameof(LogisticsClaimGoodsInfo.Status)}", statusQuery));
            }

            if (queryParam.ShelfStatusQuery != null && queryParam.ShelfStatusQuery != ExpressShelfStatus.None)
            {
                ExpressShelfStatus shelfStatusQuery = queryParam.ShelfStatusQuery;
                criteria = criteria.Add(Restrictions.Eq($"cg.{nameof(LogisticsClaimGoodsInfo.ShelfStatus)}", shelfStatusQuery));
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
                // criteria = criteria.Add(Restrictions.Eq($"{nameof(ExpressDumpScheduleInfo.ScheduleType)}", queryParam.ScheduleType)); 这个前面已经有限制条件了
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
                criteria = criteria.Add(Restrictions.Ge($"{nameof(ExpressDumpScheduleInfo.CreateTime)}", scheduleTimeBeginQueryValue));
            }

            if (queryParam.ScheduleTimeEndQuery != null)
            {
                DateTime scheduleTimeEndQueryValue = queryParam.ScheduleTimeEndQuery.Value.Date.AddDays(1);
                criteria = criteria.Add(Restrictions.Lt($"{nameof(ExpressDumpScheduleInfo.CreateTime)}", scheduleTimeEndQueryValue));
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

            return criteria;
        }
    }
}
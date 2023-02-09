using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderClientListService {
    private readonly ISession session;

    public LogisticsOrderClientListService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 获取客户端订单列表
    /// </summary>
    /// <returns>按订单id倒序排列的订单列表</returns>
    public IList<LogisticsOrderClientListItem> GetCustomerOrderList(LogisticsOrderClientListParam param) {
        ValidationUtil.Validate<LogisticsOrderClientListParamServiceValidator, LogisticsOrderClientListParam>(param);

        var listQuery = GetListQuery(param);

        if (param.LastId != ClientListLastIdModel.LAST_ID_NOT_SET_VALUE) {
            // 跟降序排序匹配
            int lastIdQuery = param.LastId;
            listQuery = listQuery.Where(e => e.Id < lastIdQuery); // 降序排序id大的会被先取得
        }

        IList<LogisticsOrderInfo> orderList = listQuery
            .OrderByDescending(e => e.Id) // 按Id降序排序
            .Take(15) // 每次下拉取15个
            .ToList();

        IList<int> orderIdList = orderList.Select(e => e.Id).ToList();
        IList<LogisticsOrderPackDataInfo> orderPackDataList = session.Query<LogisticsOrderPackDataInfo>()
            .Where(e => orderIdList.Contains(e.Order.Id))
            .ToList();

        IList<LogisticsOrderClientListItem> orderItemList = new List<LogisticsOrderClientListItem>();
        foreach (var order in orderList) {
            int ordreId = order.Id;
            var orderPackData = orderPackDataList
                .Where(e => e.Order.Id == ordreId)
                .Take(1)
                .SingleOrDefault();

            var orderItem = new LogisticsOrderClientListItem {
                Id = order.Id,
                OrderCode = order.Code,
                OrderStatus = order.OrderStatus.Name,
                GoodsValue = order.GoodsValue.ToString("#0.00"),
                ArearsAmount = order.ArrearsAmountFormula.ToString("#0.00"),
                TotalPackageCount = orderPackData?.TotalPackageCount.ToString() ?? string.Empty,
            };

            orderItemList.Add(orderItem);
        }

        return orderItemList;
    }

    /// <summary>
    /// 获取客户端订单列表订单数量
    /// </summary>
    public int GetCustomerOrderCount(LogisticsOrderClientListParam param) {
        var listQuery = GetListQuery(param);
        return listQuery.Count();
    }

    private IQueryable<LogisticsOrderInfo> GetListQuery(LogisticsOrderClientListParam param) {
        int customerId = param.CustomerId;
        var query = session.Query<LogisticsOrderInfo>().Where(e => e.Customer.Id == customerId);

        if (string.IsNullOrWhiteSpace(param.Query) == false) {
            string queryStr = param.Query.Trim();

            var expressList = session.Query<ExpressInfo>()
                .Where(e => e.Customer!.Id == customerId && e.Order != null && e.TrackingNo.Contains(queryStr))
                .ToList();

            var expressOrderIdList = expressList.Select(e => e.Order!.Id).Distinct().ToList();

            query = query.Where(e => expressOrderIdList.Contains(e.Id) || e.Code.Contains(queryStr));
        }

        if (param.Cate == LogisticsOrderStatusClientCate.Submitted) {
            query = query.Where(e => e.OrderStatus == LogisticsOrderStatus.Create || e.OrderStatus == LogisticsOrderStatus.WaitPack);
        }

        if (param.Cate == LogisticsOrderStatusClientCate.Pay) {
            query = query.Where(e => e.PayStatus != PayStatus.Paid && e.ArrearsAmountFormula > 0);
        }

        if (param.Cate == LogisticsOrderStatusClientCate.WaitingTransport) {
            query = query.Where(e => e.OrderStatus == LogisticsOrderStatus.Packed || e.OrderStatus == LogisticsOrderStatus.WaitTransfer);
        }

        if (param.Cate == LogisticsOrderStatusClientCate.Transporting) {
            query = query.Where(e => e.OrderStatus == LogisticsOrderStatus.Transferred);
        }

        if (param.Cate == LogisticsOrderStatusClientCate.Delivered) {
            query = query.Where(e => e.OrderStatus == LogisticsOrderStatus.Completed);
        }

        return query;
    }
}
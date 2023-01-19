using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Express;
using shunda.Domain.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderStorageLocService {
    /// <summary>
    /// 订单仓位信息
    /// 包括 首个仓位 和 仓位数量
    /// </summary>
    public class LogisticsOrderStorageLocDto {
        public StorageLocInfo FirstStorageLoc { get; set; } = new();
        public int StorageLocCount { get; set; }
    }

    private readonly ISession session;

    public LogisticsOrderStorageLocService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 获取订单仓位信息
    /// </summary>
    /// <param name="expressList">订单快递单列表</param>
    /// <returns></returns>
    public LogisticsOrderStorageLocDto GetOrderStorageLoc(IList<ExpressInfo> expressList) {
        if (expressList.Count == 0) {
            throw new ValidationException("快递单列表不能为空！");
        }

        IList<int> storageLocIdList = expressList.Select(e => e.StorageLoc.Id).Distinct().ToList();

        var firstStorageLoc = session.Query<StorageLocInfo>()
            .Where(e => storageLocIdList.Contains(e.Id))
            .OrderBy(e => e.Shelf.OrderNum) // 先按货架顺序排
            .ThenBy(e => e.RowNum) // 先按层排列，
            .ThenBy(e => e.ColNum) // 再按列排列
            .Take(1)
            .SingleOrDefault();
        if (firstStorageLoc == null) {
            throw new ValidationException("快递单列表仓位不存在！");
        }

        return new LogisticsOrderStorageLocDto {
            FirstStorageLoc = firstStorageLoc,
            StorageLocCount = storageLocIdList.Count,
        };
    }

    /// <summary>
    /// 获取订单仓位信息
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    public LogisticsOrderStorageLocDto GetOrderStorageLoc(int orderId) {
        var expressList = session.Query<ExpressInfo>()
            .Where(e => e.Order!.Id == orderId)
            .ToList();

        return GetOrderStorageLoc(expressList);
    }
}
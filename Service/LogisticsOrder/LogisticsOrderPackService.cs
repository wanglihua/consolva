using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Express;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderPackService {
    private readonly ISession session;
    private readonly LogisticsOrderStorageLocService logisticsOrderStorageLocService;
    private readonly ExpressLogService expressLogService;
    private readonly LogisticsOrderActionService logisticsOrderActionService;

    public LogisticsOrderPackService(ISession session,
        LogisticsOrderStorageLocService logisticsOrderStorageLocService,
        ExpressLogService expressLogService,
        LogisticsOrderActionService logisticsOrderActionService
    ) {
        this.session = session;
        this.logisticsOrderStorageLocService = logisticsOrderStorageLocService;
        this.expressLogService = expressLogService;
        this.logisticsOrderActionService = logisticsOrderActionService;
    }

    /// <summary>
    /// 客户提交订单打包
    /// 现在是订单创建时就提交打包了
    /// <param name="orderId">订单id</param>
    /// <param name="customerId">操作客户</param>
    /// </summary>
    public void SubmitPack(int orderId, int customerId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException("指定的订单不存在！");
        }

        if (order.OrderStatus != LogisticsOrderStatus.Create) {
            throw new ValidationException("订单当前状态不能提交打包！");
        }

        if (order.GoodsType == LogisticsGoodsType.Forbidden) {
            throw new ValidationException("禁止类物品订单不能提交打包! ");
        }

        var expressList = session.Query<ExpressInfo>().Where(e => e.Order!.Id == orderId).ToList();
        if (expressList.Count == 0) {
            throw new ValidationException($"订单{order.Code}的快递单列表为空, 不能提交打包!");
        }

        foreach (var express in expressList) {
            if (express.Status != ExpressStatus.Order) {
                throw new ValidationException($"快递单号{express.TrackingNo}状态不是已建单状态，不能提交打包！");
            }
        }

        IList<int> expressIdList = expressList.Select(e => e.Id).ToList();
        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();
        foreach (var express in expressList) {
            int expressId = express.Id;
            int expressGoodsCount = expressGoodsList.Where(e => e.Express.Id == expressId).Count();
            if (expressGoodsCount == 0) {
                throw new ValidationException($"快递单号{express.TrackingNo}的物品列表为空, 不能提交打包!");
            }
        }

        DateTime now = DateTime.Now;

        var orderStorageLocData = logisticsOrderStorageLocService.GetOrderStorageLoc(orderId);

        // 更新快递单状态和时间
        foreach (var express in expressList) {
            express.Status = ExpressStatus.WaitPack;
            express.SubmitPackTime = now;

            session.Update(express);
        }

        session.Flush();

        // 记录订单之前状态
        var orderStatusBefore = order.OrderStatus;

        // 更新订单状态和数据
        order.OrderStatus = LogisticsOrderStatus.WaitPack;
        order.StorageLoc = orderStorageLocData.FirstStorageLoc;
        order.StorageLocCount = orderStorageLocData.StorageLocCount;
        order.SubmitPackTime = now;

        session.Update(order);
        session.Flush();

        // 订单之后状态
        var orderStatusAfter = order.OrderStatus;

        // 快递单号日志
        foreach (var express in expressList) {
            int expressId = express.Id;
            expressLogService.CustomerLog(expressId, ExpressLogType.SubmitPack, customerId);
        }

        // 订单日志
        logisticsOrderActionService.NewActionCustomer(orderId, LogisticsOrderActionType.CustomerSubmitPack, orderStatusBefore, orderStatusAfter, customerId);
    }

    /// <summary>
    /// 判断订单是不是可以撤回打包提交 
    /// </summary>
    public bool CanRollbackSubmitPack(int orderId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        if (order.OrderStatus != LogisticsOrderStatus.WaitPack) {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 撤回订单打包提交
    /// <param name="orderId"></param>
    /// <param name="userId">操作用户</param>
    /// </summary>
    public void RollbackSubmitPack(int orderId, int userId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        if (CanRollbackSubmitPack(orderId) == false) {
            throw new ValidationException("订单当前状态不能取消打包提交！");
        }

        var expressList = session.Query<ExpressInfo>()
            .Where(e => e.Order!.Id == orderId)
            .ToList();

        foreach (var express in expressList) {
            express.Status = ExpressStatus.Order;
            express.SubmitPackTime = null;
            session.Update(express);
        }

        var orderStatusBefore = order.OrderStatus;

        order.OrderStatus = LogisticsOrderStatus.Create;
        order.SubmitPackTime = null;
        session.Update(order);

        session.Flush();

        var orderStatusAfter = order.OrderStatus;

        // 日志
        foreach (var express in expressList) {
            int expressId = express.Id;
            expressLogService.UserLog(expressId, ExpressLogType.RollbackSubmitPack, userId);
        }

        logisticsOrderActionService.NewActionUser(orderId, LogisticsOrderActionType.RollbackSubmitPack, orderStatusBefore, orderStatusAfter, userId);
    }
}
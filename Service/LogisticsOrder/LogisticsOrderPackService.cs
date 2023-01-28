using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
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

        // 快递单物品列表
        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();
        foreach (var express in expressList) {
            int expressId = express.Id;
            var expressGoods = expressGoodsList.Where(e => e.Express.Id == expressId).Take(1).SingleOrDefault();
            if (expressGoods == null) {
                throw new ValidationException($"快递单号{express.TrackingNo}的物品列表为空, 不能提交打包!");
            }
        }

        // 检查快递单物品单价
        foreach (var expressGoods in expressGoodsList) {
            if (expressGoods.Price <= 0) {
                string trackingNo = expressGoods.Express.TrackingNo;
                string goodsName = expressGoods.Goods.Name;
                throw new Exception($"快递单号 {trackingNo} 物品 {goodsName} 单价没有设置，请修改单价后继续！");
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

    /// <summary>
    /// 订单完成打包
    /// </summary>
    /// <param name="orderId">订单id</param>
    /// <param name="userId">操作用户</param>
    public void CompletePack(int orderId, int userId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        var user = session.Get<UserInfo>(userId);
        if (user == null) {
            throw new ValidationException($"指定的用户#{userId}不存在！");
        }

        if (order.OrderStatus == LogisticsOrderStatus.Packed) {
            throw new ValidationException($"订单{order.Code}已经是已打包状态！");
        }

        if (order.OrderStatus != LogisticsOrderStatus.WaitPack) {
            throw new ValidationException($"订单{order.Code}不是等待打包状态不能完成打包！");
        }

        var expressList = session.Query<ExpressInfo>().Where(e => e.Order!.Id == orderId).ToList();

        var expressIdList = expressList.Select(e => e.Id).ToList();

        // 检查快递单号打包信息
        var packedExpressIdList = session.Query<LogisticsOrderPackExpressInfo>()
            .Where(e => e.Order.Id == orderId)
            .Select(e => e.Express.Id)
            .ToList();
        var notPackedExpressIdList = expressIdList.Where(e => packedExpressIdList.Contains(e) == false).ToList();
        if (notPackedExpressIdList.Count != 0) {
            var notPackedTrackingNoList = expressList
                .Where(e => notPackedExpressIdList.Contains(e.Id))
                .Select(e => e.TrackingNo)
                .ToList();

            throw new ValidationException($"订单{order.Code} 快递单号 {string.Join(", ", notPackedTrackingNoList)} 未包含在任何箱号中，订单不能完成打包！");
        }

        // 检查快递单号状态
        var notPackStatusExpressList = expressList.Where(e => e.Status != ExpressStatus.Pack).ToList();
        if (notPackStatusExpressList.Count != 0) {
            var notPackedTrackingNoList = notPackStatusExpressList.Select(e => e.TrackingNo).ToList();
            throw new ValidationException($"订单{order.Code} 快递单号 {string.Join(", ", notPackedTrackingNoList)} 不是已打包状态，订单不能完成打包！");
        }

        // 快递单物品列表
        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();
        foreach (var express in expressList) {
            int expressId = express.Id;
            var expressGoods = expressGoodsList.Where(e => e.Express.Id == expressId).Take(1).SingleOrDefault();
            if (expressGoods == null) {
                throw new ValidationException($"订单{order.Code}快递单号{express.TrackingNo}的物品列表为空, 订单不能完成打包!");
            }
        }

        // 检查快递单物品单价
        foreach (var expressGoods in expressGoodsList) {
            if (expressGoods.Price <= 0) {
                string trackingNo = expressGoods.Express.TrackingNo;
                string goodsName = expressGoods.Goods.Name;
                throw new Exception($"快递单号 {trackingNo} 物品 {goodsName} 单价没有设置，请修改单价后继续！");
            }
        }

        // 订单状态和时间更新，快递单状态这里不用更新
        var orderStatusBefore = order.OrderStatus;

        order.OrderStatus = LogisticsOrderStatus.Packed;
        order.CompletePackTime = DateTime.Now;
        session.Update(order);
        session.Flush();

        var orderStatusAfter = order.OrderStatus;

        logisticsOrderActionService.NewActionUser(orderId, LogisticsOrderActionType.Pack, orderStatusBefore, orderStatusAfter, userId);

        // 保存打包汇总数据
        var orderPackList = session.Query<LogisticsOrderPackInfo>().Where(e => e.Order.Id == orderId).ToList();

        var orderPackData = session.Query<LogisticsOrderPackDataInfo>().Where(e => e.Order.Id == orderId).Take(1).SingleOrDefault();
        if (orderPackData == null) {
            orderPackData = new LogisticsOrderPackDataInfo {
                Order = order
            };
        }

        decimal totalActualWeight = 0;
        decimal totalVolumeWeight = 0;
        decimal totalVolume = 0;

        orderPackData.PackUser = user;

        session.SaveOrUpdate(orderPackData);
        session.Flush();

        // todo 订单计费

        // todo 发送消息
    }
}
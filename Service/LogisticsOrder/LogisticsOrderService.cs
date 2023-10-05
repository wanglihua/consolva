using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Domain.Coupon;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Service.Coupon;
using shunda.Service.Express;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderService {
    private readonly ISession session;
    private readonly CouponService couponService;
    private readonly LogisticsOrderActionService logisticsOrderActionService;
    private readonly ExpressLogService expressLogService;
    private readonly LogisticsOrderPackBoxHistoryService logisticsOrderPackBoxHistoryService;

    public LogisticsOrderService(ISession session,
        CouponService couponService,
        LogisticsOrderActionService logisticsOrderActionService,
        ExpressLogService expressLogService,
        LogisticsOrderPackBoxHistoryService logisticsOrderPackBoxHistoryService
    ) {
        this.session = session;
        this.couponService = couponService;
        this.logisticsOrderActionService = logisticsOrderActionService;
        this.expressLogService = expressLogService;
        this.logisticsOrderPackBoxHistoryService = logisticsOrderPackBoxHistoryService;
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    public void CancelOrder(LogisticsOrderCancelParam param) {
        ValidationUtil.Validate<LogisticsOrderCancelParamValidator, LogisticsOrderCancelParam>(param);

        int orderId = param.OrderId;
        bool backend = param.Backend;
        int customerId = param.CustomerId;
        int userId = param.UserId;

        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        if (backend == false) {
            if (order.Customer.Id != customerId) {
                throw new ValidationException("指定所属客户与指定的客户不一致！");
            }

            var orderStatusListBeforeSubmitPack = LogisticsOrderStatus.GetBeforeSubmitPackStatusList();
            if (orderStatusListBeforeSubmitPack.Contains(order.OrderStatus) == false) {
                throw new ValidationException("订单已提交打包或后续状态，不能取消订单！");
            }

            if (order.PaidAmount > 0) {
                throw new ValidationException("订单已有支付金额，不能取消订单！");
            }

            // 只能一个小时后客户才能取消
            if (order.CreateTime.AddHours(1) > DateTime.Now) {
                throw new ValidationException("订单取消，客户只能在订单创建一个小时后操作！");
            }
        } else {
            var orderStausListAdminCanCancel = LogisticsOrderStatus.GetAdminCanCancelStatusList();
            if (orderStausListAdminCanCancel.Contains(order.OrderStatus) == false) {
                throw new ValidationException("当前订单状态后台不能取消订单！");
            }
        }

        // 更新订单状态
        order.OrderStatus = LogisticsOrderStatus.Canceled;
        session.Update(order);
        session.Flush();

        var expressList = session.Query<ExpressInfo>()
            .Where(e => e.Order!.Id == orderId)
            .ToList();

        // 更新快递单状态
        foreach (var express in expressList) {
            express.Order = null;
            express.OrderCode = string.Empty;
            express.FirstMode = null;
            express.FirstType = LogisticsModeType.None;
            express.Status = ExpressStatus.Checkin;
            session.Update(express);
        }

        session.Flush();

        // 如果有使用优惠券的，把优惠券还回去
        var couponId = session.Query<CouponInfo>()
            .Where(e => e.Order!.Id == orderId)
            .Select(e => e.Id)
            .Take(1) // 一个订单只能使用一张优惠券
            .SingleOrDefault();
        if (couponId != 0) {
            couponService.SetCouponNotUsedAndSumSaveBatchTotalUsed(couponId);
        }

        // 订单日志
        if (backend == false) {
            logisticsOrderActionService.NewActionCustomer(orderId, LogisticsOrderActionType.CustomerCancelOrder, customerId);
        } else {
            logisticsOrderActionService.NewActionUser(orderId, LogisticsOrderActionType.UserCancelOrder, userId);
        }

        // 快递单日志
        foreach (var express in expressList) {
            int expressId = express.Id;

            if (backend == false) {
                expressLogService.CustomerLog(expressId, ExpressLogType.CancelOrder, customerId);
            } else {
                expressLogService.UserLog(expressId, ExpressLogType.CancelOrder, userId);
            }
        }
    }

    /// <summary>
    /// 完成订单
    /// </summary>
    public void CompleteOrder(LogisticsOrderCompleteParam param) {
        ValidationUtil.Validate<LogisticsOrderCompleteParamValidator, LogisticsOrderCompleteParam>(param);

        int orderId = param.OrderId;
        int userId = param.UserId;

        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        UserInfo? user = null;
        if (userId != 0) {
            user = session.Get<UserInfo>(userId);
            if (user == null) {
                throw new ValidationException($"指定的用户#{userId}不存在！");
            }
        }

        if (order.OrderStatus == LogisticsOrderStatus.Completed) {
            throw new ValidationException($"指定的订单#{orderId}当前已经是完结状态！");
        }

        var orderStatusListAdminCanComplete = LogisticsOrderStatus.GetAdminCanCompleteStatusList();
        if (orderStatusListAdminCanComplete.Contains(order.OrderStatus) == false) {
            throw new ValidationException("当前订单状态后台不能完成订单！");
        }

        // 更新订单状态
        order.OrderStatus = LogisticsOrderStatus.Completed;
        session.Update(order);

        if (user == null) {
            logisticsOrderActionService.NewActionWithoutUserCustomer(orderId, LogisticsOrderActionType.DeliverComplete);
        } else {
            logisticsOrderActionService.NewActionUser(orderId, LogisticsOrderActionType.DeliverComplete, userId);
        }

        session.Flush();

        // 更新快递单状态
        var expressList = session.Query<ExpressInfo>()
            .Where(e => e.Order!.Id == orderId)
            .ToList();

        foreach (var express in expressList) {
            if (express.Status != ExpressStatus.Complete) {
                express.Status = ExpressStatus.Complete;
                session.Update(express);
            }
        }

        session.Flush();

        // 更新订单打包状态
        var orderPackBoxList = session.Query<LogisticsOrderPackBoxInfo>()
            .Where(e => e.Order.Id == orderId)
            .ToList();
        foreach (var packBox in orderPackBoxList) {
            if (packBox.Status != LogisticsOrderPackBoxStatus.Deliver) {
                var packBoxStatusBefore = packBox.Status;
                packBox.Status = LogisticsOrderPackBoxStatus.Deliver;
                session.Update(packBox);

                var packBoxStatusAfter = packBox.Status;

                logisticsOrderPackBoxHistoryService.AddHistory(packBox, LogisticsOrderPackBoxHistoryType.Deliver, packBoxStatusBefore, packBoxStatusAfter, userId);
            }
        }

        session.Flush();
    }
}
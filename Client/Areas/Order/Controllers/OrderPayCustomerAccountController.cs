using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderPayCustomerAccount;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Coupon;
using shunda.Service.Customer;
using shunda.Service.Fee;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderPayCustomerAccountController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;
    private readonly LogisticsOrderActionService logisticsOrderActionService;
    private readonly CustomerScoreService customerScoreService;
    private readonly CustomerAccountService customerAccountService;
    private readonly CouponService couponService;
    private readonly LogisticsOrderFeeSumService logisticsOrderFeeSumService;

    public OrderPayCustomerAccountController(
        ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsOrderPaySumService logisticsOrderPaySumService,
        LogisticsOrderActionService logisticsOrderActionService,
        CustomerScoreService customerScoreService,
        CustomerAccountService customerAccountService,
        CouponService couponService,
        LogisticsOrderFeeSumService logisticsOrderFeeSumService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
        this.logisticsOrderActionService = logisticsOrderActionService;
        this.customerScoreService = customerScoreService;
        this.customerAccountService = customerAccountService;
        this.couponService = couponService;
        this.logisticsOrderFeeSumService = logisticsOrderFeeSumService;
    }

    [HttpPost]
    [Route("/client/order/pay/customer/account/detail")]
    public IActionResult Detail([FromBody] OrderPayCustomerAccountDetailParam param) {
        ValidationUtil.Validate<OrderPayCustomerAccountDetailParamValidator, OrderPayCustomerAccountDetailParam>(param);

        int orderId = param.OrderId;
        int couponId = param.CouponId;

        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        CouponInfo? coupon = null;
        if (couponId > 0) {
            coupon = session.Get<CouponInfo>(couponId);
            if (coupon == null) {
                throw new ValidationException($"指定的优惠券{couponId}不存在！");
            }
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var loginCustomer = loginCustomerService.GetLoginCustomer();

        if (order.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的订单{order.Code}不属于当前登录客户#{loginCustomerId}！");
        }

        logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(orderId);

        var orderFeeSumData = logisticsOrderFeeSumService.GetSumData(orderId);

        decimal discountAmount = 0;
        string discountAmountStr = string.Empty;
        if (coupon != null) {
            var discountResult = couponService.CalculateCouponDiscount(couponId, orderFeeSumData.FirstModeFee);
            if (discountResult.Success == false) {
                discountAmount = 0;
                discountAmountStr = discountResult.Message;
            } else {
                discountAmount = discountResult.Discount;
                discountAmountStr = discountAmount.ToString("#0.00");
            }
        }

        decimal paidAmount = order.PaidAmount;
        decimal arrearsAmount = orderFeeSumData.TotalAmountWithoutCouponDiscount - discountAmount - paidAmount;

        if (arrearsAmount < 0) {
            arrearsAmount = 0;
        }

        decimal customerAccount = loginCustomer.Account;

        // 这里不提示
        // if (customerAccount <= 0) {
        //     throw new ValidationException("当前客户账户没有余额，不能支付！");
        // }

        decimal actualPayAmount = arrearsAmount;
        if (customerAccount < arrearsAmount) {
            actualPayAmount = customerAccount;
        }

        // 这个判断一般是不会成立的
        if (actualPayAmount < 0) {
            actualPayAmount = 0;
        }

        bool hasAccountToPay = customerAccount > 0;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderPayCustomerAccountDetailModel {
                OrderId = order.Id,
                OrderCode = order.Code,
                TotalAmount = orderFeeSumData.TotalAmountWithoutCouponDiscount.ToString("#0.00"),
                HasCoupon = coupon != null,
                DiscountAmount = discountAmountStr,
                PaidAmount = paidAmount.ToString("#0.00"),
                ArrearsAmount = arrearsAmount.ToString("#0.00"),
                ActualPayAmount = actualPayAmount.ToString("#0.00"),
                HasAccountToPay = hasAccountToPay,
            },
        });
    }

    [HttpPost]
    [Route("/client/order/pay/customer/account/pay")]
    [Transactional]
    public IActionResult Pay([FromBody] OrderPayCustomerAccountPayParam param) {
        ValidationUtil.Validate<OrderPayCustomerAccountPayParamValidator, OrderPayCustomerAccountPayParam>(param);

        int orderId = param.OrderId;
        int couponId = param.CouponId;

        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        CouponInfo? coupon = null;
        if (couponId > 0) {
            coupon = session.Get<CouponInfo>(couponId);
            if (coupon == null) {
                throw new ValidationException($"指定的优惠券{couponId}不存在！");
            }
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var loginCustomer = loginCustomerService.GetLoginCustomer();

        if (order.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的订单{order.Code}不属于当前登录客户#{loginCustomerId}！");
        }

        var orderFeeSumData = logisticsOrderFeeSumService.GetSumData(orderId);

        decimal discountAmount = 0;
        if (coupon != null) {
            var discountResult = couponService.CalculateCouponDiscount(couponId, orderFeeSumData.FirstModeFee);
            if (discountResult.Success == false) {
                discountAmount = 0;
            } else {
                discountAmount = discountResult.Discount;

                // 保存优惠券到订单，并生成响应的订单费用，负的费用
                couponService.SaveToOrder(customerId: loginCustomerId, orderId: orderId, couponId: couponId);
            }
        }

        decimal paidAmount = order.PaidAmount;
        decimal arrearsAmount = orderFeeSumData.TotalAmountWithoutCouponDiscount - discountAmount - paidAmount;

        if (arrearsAmount < 0) {
            arrearsAmount = 0;
        }

        decimal customerAccount = loginCustomer.Account;

        decimal actualPayAmount = arrearsAmount;
        if (customerAccount < arrearsAmount) {
            actualPayAmount = customerAccount;
        }

        // 这个判断一般是不会成立的
        if (actualPayAmount < 0) {
            actualPayAmount = 0;
        }

        PayStatus payStatusBefore = order.PayStatus;

        var orderPay = new LogisticsOrderPayInfo {
            Order = order,
            OutTradeNo = string.Empty, // 非空，其实生成一个也可以的
            Customer = order.Customer,
            PayType = PayType.Balance,
            Amount = actualPayAmount, // 特殊情况的时候：订单用户原来支付过了一些，第二次支付的时候获得了优惠券折扣，这个折扣正好完全抵掉了欠款，这时就会生成一个支付金额为0的记录，同时把订单标记为已支付。
            Status = PayStatus.Paid
        };

        session.Save(orderPay);
        session.Flush();

        logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(orderId);

        order.PayType = PayType.Balance;
        order.PayTime = DateTime.Now;

        session.Update(order);
        session.Flush();

        PayStatus payStatusAfter = order.PayStatus;

        customerAccountService.Change(CustomerAccountChangeType.Pay, loginCustomer.Id, -actualPayAmount, $"余额支付订单：{order.Code}");

        // 会员积分
        int orderPayId = orderPay.Id;
        customerScoreService.AddScore(orderPayId);

        logisticsOrderActionService.NewActionCustomer(orderId, LogisticsOrderActionType.Pay, payStatusBefore, payStatusAfter, $"余额支付，金额：{actualPayAmount:#0.00)}", loginCustomerId);

        return Json(new JsonResultData {
            Success = true,
            Message = "支付成功！",
        });
    }
}
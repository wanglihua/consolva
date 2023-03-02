using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderPayCustomerAccount;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
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

    public OrderPayCustomerAccountController(
        ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsOrderPaySumService logisticsOrderPaySumService,
        LogisticsOrderActionService logisticsOrderActionService,
        CustomerScoreService customerScoreService,
        CustomerAccountService customerAccountService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
        this.logisticsOrderActionService = logisticsOrderActionService;
        this.customerScoreService = customerScoreService;
        this.customerAccountService = customerAccountService;
    }

    [HttpPost]
    [Route("/client/order/pay/customer/account/detail")]
    public IActionResult Detail([FromBody] OrderPayCustomerAccountDetailParam param) {
        ValidationUtil.Validate<OrderPayCustomerAccountDetailParamValidator, OrderPayCustomerAccountDetailParam>(param);

        int orderId = param.OrderId;
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var loginCustomer = loginCustomerService.GetLoginCustomer();

        if (order.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的订单{order.Code}不属于当前登录客户#{loginCustomerId}！");
        }

        decimal arrearsAmount = (order.OrderAmount - order.PaidAmount);

        if (arrearsAmount <= 0) {
            throw new ValidationException("订单没有欠费。无需支付！");
        }

        decimal customerAccount = loginCustomer.Account;

        if (customerAccount <= 0) {
            throw new ValidationException("当前客户账户没有余额，不能支付！");
        }

        decimal actualPayAmount = arrearsAmount;
        if (customerAccount < arrearsAmount) {
            actualPayAmount = customerAccount;
        }

        // 这个判断一般是不会成立的s
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
                TotalAmount = order.OrderAmount.ToString("#0.00"),
                PaidAmount = order.PaidAmount.ToString("#0.00"),
                ArrearsAmount = arrearsAmount.ToString("#0.00"), // 这里不能用 ArrearsAmountFormula
                ActualPayAmount = actualPayAmount.ToString("#0.00"),
                HasAccountToPay = hasAccountToPay,
            },
        });
    }

    [HttpPost]
    [Route("/client/order/pay/customer/account/pay")]
    public IActionResult Pay([FromBody] OrderPayCustomerAccountPayParam param) {
        ValidationUtil.Validate<OrderPayCustomerAccountPayParamValidator, OrderPayCustomerAccountPayParam>(param);

        int orderId = param.OrderId;
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var loginCustomer = loginCustomerService.GetLoginCustomer();

        if (order.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的订单{order.Code}不属于当前登录客户#{loginCustomerId}！");
        }

        decimal arrearsAmount = (order.OrderAmount - order.PaidAmount);

        if (arrearsAmount <= 0) {
            throw new ValidationException("订单没有欠费。无需支付！");
        }

        decimal customerAccount = loginCustomer.Account;

        if (customerAccount <= 0) {
            throw new ValidationException("当前客户账户没有余额，不能支付！");
        }

        decimal actualPayAmount = arrearsAmount;
        if (customerAccount < arrearsAmount) {
            actualPayAmount = customerAccount;
        }

        // 这个判断一般是不会成立的
        if (customerAccount < 0) {
            actualPayAmount = 0;
        }

        PayStatus payStatusBefore = order.PayStatus;

        var orderPay = new LogisticsOrderPayInfo {
            Order = order,
            OutTradeNo = string.Empty, // 非空，其实生成一个也可以的
            Customer = order.Customer,
            PayType = PayType.Balance,
            Amount = actualPayAmount,
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

        int orderPayId = orderPay.Id;

        customerAccountService.Change(CustomerAccountChangeType.Pay, loginCustomer.Id, -actualPayAmount, $"余额支付订单：{order.Code}");

        // 会员积分
        customerScoreService.AddScore(orderPayId);

        logisticsOrderActionService.NewActionCustomer(orderId, LogisticsOrderActionType.Pay, payStatusBefore, payStatusAfter, $"余额支付，金额：{actualPayAmount:#0.00)}", loginCustomerId);

        return Json(new JsonResultData {
            Success = true,
            Message = "支付成功！",
        });
    }
}
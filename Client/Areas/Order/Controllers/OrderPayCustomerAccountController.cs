using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderPayCustomerAccount;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderPayCustomerAccountController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public OrderPayCustomerAccountController(
        ISession session,
        LoginCustomerService loginCustomerService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
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
        decimal customerAccount = loginCustomer.Account;

        decimal actualPayAmount = arrearsAmount;
        if (customerAccount < arrearsAmount) {
            actualPayAmount = customerAccount;
        }

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
            },
        });
    }

    [HttpPost]
    [Route("/client/order/pay/customer/account/pay")]
    public IActionResult Pay() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}
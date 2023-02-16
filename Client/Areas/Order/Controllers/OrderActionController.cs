using Microsoft.AspNetCore.Mvc;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderActionController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsOrderService logisticsOrderService;

    public OrderActionController(ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsOrderService logisticsOrderService) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsOrderService = logisticsOrderService;
    }

    [HttpPost]
    [Route("/client/order/action/cancel")]
    public IActionResult Cancel(int orderId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        if (order.Customer.Id != loginCustomerId) {
            throw new ValidationException($"订单{order.Code}不属于当前登录客户！");
        }

        logisticsOrderService.CancelOrder(new LogisticsOrderCancelParam {
            OrderId = orderId,
            Backend = false,
            CustomerId = loginCustomerId,
        });

        return Json(new JsonResultData {
            Success = true,
            Message = "订单取消成功！",
        });
    }
}
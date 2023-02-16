using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderDetail;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderDetailController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public OrderDetailController(ISession session, LoginCustomerService loginCustomerService) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/order/detail")]
    public IActionResult Detail(int orderId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        if (order.Customer.Id != loginCustomerId) {
            throw new ValidationException($"订单{order.Code}不属于当前登录客户！");
        }

        var model = new OrderDetailModel {

        };

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }
}
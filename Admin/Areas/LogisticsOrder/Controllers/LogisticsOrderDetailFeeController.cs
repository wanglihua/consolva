using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetailFee;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

[LogisticsOrderArea]
[AdminAuth(PassRes.LogisticsOrder.OrderListActive, PassRes.LogisticsOrder.OrderListComplete, PassRes.LogisticsOrder.OrderListCancel)]
public class LogisticsOrderDetailFeeController : Controller {
    private readonly ISession session;
    private readonly LogisticsOrderFeeService logisticsOrderFeeService;

    public LogisticsOrderDetailFeeController(
        ISession session,
        LogisticsOrderFeeService logisticsOrderFeeService
    ) {
        this.session = session;
        this.logisticsOrderFeeService = logisticsOrderFeeService;
    }

    [HttpPost]
    [Route("/admin/logistics/order/detail/re/calculate/order/fee")]
    [Transactional]
    public IActionResult ReCalculateOrderFee(int orderId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        if (order.OrderStatus == LogisticsOrderStatus.Completed) {
            throw new ValidationException($"订单{order.Code}已交付，不能重新计费！");
        }

        logisticsOrderFeeService.CalculateAndSaveFee(orderId);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "重新计算费用并保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/order/detail/fee/list")]
    public IActionResult List(int orderId) {
        var model = new LogisticsOrderDetailFeeListModel {
            OrderId = orderId
        };

        return View("LogisticsOrderDetailFeeList", model);
    }
}
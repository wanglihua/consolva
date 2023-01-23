using Microsoft.AspNetCore.Mvc;
using NHibernate;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetail;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderList;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

[LogisticsOrderArea]
[AdminAuth(PassRes.LogisticsOrder.OrderListActive, PassRes.LogisticsOrder.OrderListComplete, PassRes.LogisticsOrder.OrderListCancel)]
public class LogisticsOrderDetailController : Controller {
    private readonly ISession session;

    public LogisticsOrderDetailController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/order/detail/{orderId:int}")]
    public IActionResult Index(int orderId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        NHibernateUtil.Initialize(order.Customer);
        NHibernateUtil.Initialize(order.Insurance);
        NHibernateUtil.Initialize(order.Gst);

        // 订单列表菜单类型
        OrderListMenuType menuType = OrderListMenuType.Active;
        if (order.OrderStatus == LogisticsOrderStatus.Completed) {
            menuType = OrderListMenuType.Complete;
        }

        if (order.OrderStatus == LogisticsOrderStatus.Canceled) {
            menuType = OrderListMenuType.Cancel;
        }

        // 订单备注
        var logisticsOrderRemarkList = session.Query<LogisticsOrderRemarkInfo>()
            .Where(e => e.Order.Id == orderId)
            .OrderBy(e => e.Id)
            .ToList();

        var model = new LogisticsOrderDetailIndexModel {
            MenuType = menuType,
            LogisticsOrder = order,
            LogisticsOrderRemarkList = logisticsOrderRemarkList,
        };

        return View("LogisticsOrderDetailIndex", model);
    }
}
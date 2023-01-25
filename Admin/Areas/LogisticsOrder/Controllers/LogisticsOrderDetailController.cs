using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetail;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderList;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Express;
using shunda.Service.LogisticsOrder;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

[LogisticsOrderArea]
[AdminAuth(PassRes.LogisticsOrder.OrderListActive, PassRes.LogisticsOrder.OrderListComplete, PassRes.LogisticsOrder.OrderListCancel)]
public class LogisticsOrderDetailController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly LogisticsOrderRemarkService logisticsOrderRemarkService;
    private readonly ExpressService expressService;
    private readonly PassResService passResService;

    public LogisticsOrderDetailController(ISession session,
        LoginUserService loginUserService,
        LogisticsOrderRemarkService logisticsOrderRemarkService,
        ExpressService expressService,
        PassResService passResService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.logisticsOrderRemarkService = logisticsOrderRemarkService;
        this.expressService = expressService;
        this.passResService = passResService;
    }

    [HttpGet]
    [Route("/admin/logistics/order/detail/{id:int}")]
    public IActionResult Index(int id) {
        int orderId = id;
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        int loginUserId = loginUserService.GetLoginUserId();

        NHibernateUtil.Initialize(order.Customer);
        NHibernateUtil.Initialize(order.Warehouse);
        NHibernateUtil.Initialize(order.FirstMode);
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
        var logisticsOrderRemarkList = logisticsOrderRemarkService.GetOrderRemarkList(orderId);

        // 收货地址
        var receiveAddress = session.Query<LogisticsOrderReceiveAddressInfo>()
            .Where(e => e.Order.Id == orderId)
            .Take(1)
            .SingleOrDefault();
        if (receiveAddress == null) {
            throw new ValidationException("订单收货地址缺失！");
        }

        // 发货地址
        var sendAddress = session.Query<LogisticsOrderSendAddressInfo>()
            .Fetch(e => e.Province)
            .Fetch(e => e.City)
            .Fetch(e => e.Area)
            .Fetch(e => e.Street)
            .Where(e => e.Order.Id == orderId)
            .Take(1)
            .SingleOrDefault();
        if (sendAddress == null) {
            throw new ValidationException("订单发货地址缺失！");
        }

        // 订单商品
        var expressList = expressService.GetExpressListForOrderDetail(orderId);

        // 订单操作日志
        var orderActionList = session.Query<LogisticsOrderActionInfo>()
            .Fetch(e => e.User)
            .Where(e => e.Order.Id == orderId)
            .OrderByDescending(e => e.Id)
            .ToList();

        var model = new LogisticsOrderDetailIndexModel {
            MenuType = menuType,
            LogisticsOrder = order,
            OrderRemarkList = logisticsOrderRemarkList,
            ReceiveAddress = receiveAddress,
            SendAddress = sendAddress,
            ExpressList = expressList,
            LogisticsOrderActionList = orderActionList,

            CanOrderRemarkEdit = passResService.UserHasPassRes(loginUserId, PassRes.LogisticsOrder.OrderRemarkEdit),
        };

        return View("LogisticsOrderDetailIndex", model);
    }
}
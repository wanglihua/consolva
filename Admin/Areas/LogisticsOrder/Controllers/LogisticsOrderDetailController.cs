using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetail;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderList;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Fee;
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
    private readonly ExpressRemarkService expressRemarkService;
    private readonly ExpressService expressService;
    private readonly PassResService passResService;
    private readonly LogisticsOrderPackBoxService logisticsOrderPackBoxService;

    public LogisticsOrderDetailController(ISession session,
        LoginUserService loginUserService,
        LogisticsOrderRemarkService logisticsOrderRemarkService,
        ExpressRemarkService expressRemarkService,
        ExpressService expressService,
        PassResService passResService,
        LogisticsOrderPackBoxService logisticsOrderPackBoxService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.logisticsOrderRemarkService = logisticsOrderRemarkService;
        this.expressRemarkService = expressRemarkService;
        this.expressService = expressService;
        this.passResService = passResService;
        this.logisticsOrderPackBoxService = logisticsOrderPackBoxService;
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
        var orderRemarkList = logisticsOrderRemarkService.GetOrderRemarkList(orderId);

        // 快递单备注
        var expressRemarkList = expressRemarkService.GetOrderExpressRemarkList(orderId);

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

        // 订单头程物流，打包信息，箱号
        IList<LogisticsOrderFirstModeInfo> orderFirstModeList = session.Query<LogisticsOrderFirstModeInfo>()
            .Where(e => e.Order.Id == orderId)
            .ToList();

        IList<LogisticsOrderPackInfo> orderPackList = session.Query<LogisticsOrderPackInfo>()
            .Where(e => e.Order.Id == orderId)
            .ToList();

        var orderPackExpressList = session.Query<LogisticsOrderPackExpressInfo>()
            .Fetch(e => e.Express)
            .Where(e => e.Order.Id == orderId)
            .ToList();

        var expressIdList = orderPackExpressList.Select(e => e.Express.Id).ToList();
        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();

        IList<LogisticsOrderPackBoxInfo> orderPackBoxList = session.Query<LogisticsOrderPackBoxInfo>()
            .Where(e => e.Order.Id == orderId)
            .OrderBy(e => e.Id)
            .ToList();

        foreach (var packBox in orderPackBoxList) {
            packBox.CodeCache = logisticsOrderPackBoxService.GetBoxCode(packBox.Id);
        }

        foreach (var orderPack in orderPackList) {
            int orderPackId = orderPack.Id;

            var boxListOfPack = orderPackBoxList.Where(e => e.OrderPack.Id == orderPackId).OrderBy(e => e.Id).ToList();
            orderPack.PackBoxCodeListCache = boxListOfPack.Select(e => e.CodeCache).ToList();

            var orderPackExpressListOfPack = orderPackExpressList.Where(e => e.OrderPack.Id == orderPackId).ToList();
            var expressListOfPack = orderPackExpressListOfPack.Select(e => e.Express).ToList();
            orderPack.TrackingNoListCache = expressListOfPack.Select(e => e.TrackingNo).ToList();

            var expressIdListOfPack = expressListOfPack.Select(e => e.Id).ToList();
            var expressGoodsListOfPack = expressGoodsList.Where(e => expressIdListOfPack.Contains(e.Express.Id)).ToList();
            orderPack.GoodsNameListCache = expressGoodsListOfPack.Select(e => e.Goods.Name).Distinct().ToList();
        }

        foreach (var orderFirstMode in orderFirstModeList) {
            int orderFirstModeId = orderFirstMode.Id;

            orderFirstMode.OrderPackListCache = orderPackList
                .Where(e => e.OrderFirstMode.Id == orderFirstModeId)
                .OrderBy(e => e.Id)
                .ToList();
        }

        // 订单费用
        var orderFeeList = session.Query<LogisticsOrderFeeInfo>()
            .Where(e => e.Order.Id == orderId)
            .OrderBy(e => e.Id)
            .ToList();

        // 订单操作日志
        var orderActionList = session.Query<LogisticsOrderActionInfo>()
            .Fetch(e => e.User)
            .Where(e => e.Order.Id == orderId)
            .OrderByDescending(e => e.Id)
            .ToList();

        var model = new LogisticsOrderDetailIndexModel {
            MenuType = menuType,
            Order = order,
            OrderRemarkList = orderRemarkList,
            ExpressRemarkList = expressRemarkList,
            ReceiveAddress = receiveAddress,
            SendAddress = sendAddress,
            ExpressList = expressList,
            OrderFirstModeList = orderFirstModeList,
            OrderPackBoxList = orderPackBoxList,
            OrderFeeList = orderFeeList,
            LogisticsOrderActionList = orderActionList,

            CanOrderRemarkEdit = passResService.UserHasOneOfPassRes(loginUserId, PassRes.LogisticsOrder.OrderRemarkEdit, PassRes.ReceivePack.OrderPack),
        };

        return View("LogisticsOrderDetailIndex", model);
    }
}
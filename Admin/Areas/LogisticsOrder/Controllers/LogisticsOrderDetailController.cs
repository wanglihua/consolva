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
    private readonly ExpressRemarkService expressRemarkService;
    private readonly ExpressService expressService;
    private readonly PassResService passResService;

    public LogisticsOrderDetailController(ISession session,
        LoginUserService loginUserService,
        LogisticsOrderRemarkService logisticsOrderRemarkService,
        ExpressRemarkService expressRemarkService,
        ExpressService expressService,
        PassResService passResService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.logisticsOrderRemarkService = logisticsOrderRemarkService;
        this.expressRemarkService = expressRemarkService;
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

        IList<LogisticsOrderFirstModeInfo> orderFirstModeList = session.Query<LogisticsOrderFirstModeInfo>()
            .Where(e => e.Order.Id == orderId)
            .ToList();

        IList<LogisticsOrderPackInfo> orderPackList = session.Query<LogisticsOrderPackInfo>()
            .Where(e => e.Order.Id == orderId)
            .ToList();

        IList<LogisticsOrderPackBoxInfo> orderPackBoxList = session.Query<LogisticsOrderPackBoxInfo>()
            .Where(e => e.Order.Id == orderId)
            .ToList();
        
        

        /*
         
            IList<LogisticsOrderPackInfo> orderPackList = session.Query<LogisticsOrderPackInfo>().Where(e => e.Order.Id == orderId).OrderBy(e => e.GoodsType).ThenBy(e => e.Id).ToList();
            var orderPackExpressList = session.Query<LogisticsOrderPackExpressInfo>().Fetch(e => e.OrderExpress).Where(e => e.Order.Id == orderId).ToList();
            foreach (LogisticsOrderPackInfo pack in orderPackList)
            {
                var boxNumList = session.Query<LogisticsOrderPackBoxInfo>().Where(e => e.OrderPack.Id == pack.Id).Select(e => e.OrderPackBoxNum).ToList();
                pack.BoxNumStr = string.Join(",", boxNumList);

                var packExpressTrackingNoList = orderPackExpressList.Where(e => e.OrderPack.Id == pack.Id).Select(e => e.OrderExpress.ExpressTrackingNo).ToList();
                pack.ExpressTrackingNos = string.Join(",", packExpressTrackingNoList);

                var expressIdList = orderPackExpressList.Where(e => e.OrderPack.Id == pack.Id).Select(e => e.OrderExpress.Id).ToList();
                var orderPackGoodsList = orderGoodsList.Where(e => expressIdList.Contains(e.Express.Id)).ToList();

                var goodsNameList = new List<string>();
                foreach (var orderPackGoods in orderPackGoodsList)
                {
                    string goodsName = string.Empty;
                    if (orderPackGoods.GoodsAdminId.HasValue && orderPackGoods.GoodsAdminId.Value != 0)
                    {
                        var goodsAdminId = orderPackGoods.GoodsAdminId.Value;
                        goodsName = goodsList.Where(e => e.Id == goodsAdminId).Take(1).SingleOrDefault()?.GetGoodsAdminName();
                    }

                    if (string.IsNullOrWhiteSpace(goodsName))
                    {
                        goodsName = goodsList.Where(e => e.Id == orderPackGoods.GoodsId).Take(1).SingleOrDefault()?.Name;
                    }

                    if (goodsNameList.Contains(goodsName) == false)
                    {
                        goodsNameList.Add(goodsName);
                    }
                }

                pack.GoodsNames = string.Join(",", goodsNameList);
            }

            IList<LogisticsOrderDetailGoodsTypePack> goodsTypePackList = new List<LogisticsOrderDetailGoodsTypePack>();
            IList<LogisticsGoodsType> goodsTypeList = LogisticsGoodsType.GetInnerList();
            foreach (LogisticsGoodsType goodsType in goodsTypeList)
            {
                IList<LogisticsOrderPackInfo> packList = orderPackList.Where(e => e.GoodsType == goodsType).ToList();

                if (packList.Count > 0)
                {
                    LogisticsOrderDetailGoodsTypePack goodsTypePack = new LogisticsOrderDetailGoodsTypePack();

                    goodsTypePack.GoodsType = goodsType;
                    goodsTypePack.TotalVolume = packList.Sum(e => e.Length * e.Width * e.Height * e.PackageCount);
                    goodsTypePack.TotalWeight = packList.Sum(e => e.Weight * e.PackageCount);
                    goodsTypePack.TotalVolumeWeight = packList.Sum(e => e.Length * e.Width * e.Height * e.PackageCount) / logisticsFeeRateService.GetVolumeDenominator();
                    goodsTypePack.PackCount = packList.Sum(e => e.PackageCount);

                    goodsTypePack.PackList = packList;

                    goodsTypePackList.Add(goodsTypePack);
                }
            }         
         
         */


        // 订单操作日志
        var orderActionList = session.Query<LogisticsOrderActionInfo>()
            .Fetch(e => e.User)
            .Where(e => e.Order.Id == orderId)
            .OrderByDescending(e => e.Id)
            .ToList();

        var model = new LogisticsOrderDetailIndexModel {
            MenuType = menuType,
            LogisticsOrder = order,
            OrderRemarkList = orderRemarkList,
            ExpressRemarkList = expressRemarkList,
            ReceiveAddress = receiveAddress,
            SendAddress = sendAddress,
            ExpressList = expressList,
            LogisticsOrderActionList = orderActionList,

            CanOrderRemarkEdit = passResService.UserHasOneOfPassRes(loginUserId, PassRes.LogisticsOrder.OrderRemarkEdit, PassRes.ReceivePack.OrderPack),
        };

        return View("LogisticsOrderDetailIndex", model);
    }
}
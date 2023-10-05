using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using Quartz.Xml.JobSchedulingData20;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetail;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderList;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Express;
using shunda.Service.Logistics;
using shunda.Service.LogisticsOrder;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

[LogisticsOrderArea]
[AdminAuth(PassRes.LogisticsOrder.OrderListActive, PassRes.LogisticsOrder.OrderListComplete, PassRes.LogisticsOrder.OrderListCancel)]
public class LogisticsOrderDetailController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly ExpressService expressService;
    private readonly PassResService passResService;
    private readonly LogisticsOrderPackBoxService logisticsOrderPackBoxService;
    private readonly ExpressPhotoFileService expressPhotoFileService;
    private readonly ExpressSizeWeightService expressSizeWeightService;
    private readonly LogisticsModeSettingService logisticsModeSettingService;
    private readonly LogisticsOrderService logisticsOrderService;

    public LogisticsOrderDetailController(ISession session,
        LoginUserService loginUserService,
        ExpressService expressService,
        PassResService passResService,
        LogisticsOrderPackBoxService logisticsOrderPackBoxService,
        ExpressPhotoFileService expressPhotoFileService,
        ExpressSizeWeightService expressSizeWeightService,
        LogisticsModeSettingService logisticsModeSettingService,
        LogisticsOrderService logisticsOrderService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.expressService = expressService;
        this.passResService = passResService;
        this.logisticsOrderPackBoxService = logisticsOrderPackBoxService;
        this.expressPhotoFileService = expressPhotoFileService;
        this.expressSizeWeightService = expressSizeWeightService;
        this.logisticsModeSettingService = logisticsModeSettingService;
        this.logisticsOrderService = logisticsOrderService;
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

        // 收货地址
        var receiveAddress = session.Query<LogisticsOrderReceiveAddressInfo>()
            .Where(e => e.Order.Id == orderId)
            .Take(1)
            .SingleOrDefault();
        if (receiveAddress == null) {
            throw new ValidationException("订单收货地址缺失！");
        }

        /*
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
        */

        // 订单商品
        var expressList = expressService.GetExpressListForOrderDetail(orderId);

        // 快递单尺寸重量
        IList<string> trackingNoList = expressList.Select(e => e.TrackingNo).ToList();
        var expressSizeWeightList = expressSizeWeightService.GetSizeWeightList(trackingNoList);

        // 头程物流
        var orderFirstModeList = session.Query<LogisticsOrderFirstModeInfo>()
            .Fetch(e => e.FirstMode)
            .Where(e => e.Order.Id == orderId).ToList();

        // 箱号
        IList<LogisticsOrderPackBoxInfo> orderPackBoxList = session.Query<LogisticsOrderPackBoxInfo>()
            .Where(e => e.Order.Id == orderId)
            .OrderBy(e => e.Id)
            .ToList();
        foreach (var packBox in orderPackBoxList) {
            packBox.CodeCache = logisticsOrderPackBoxService.GetBoxCode(packBox.Id);
        }

        // 订单操作日志
        var orderActionList = session.Query<LogisticsOrderActionInfo>()
            .Fetch(e => e.User)
            .Where(e => e.Order.Id == orderId)
            .OrderByDescending(e => e.Id)
            .ToList();

        LogisticsRepackType repackType = order.RepackType;
        var repackTypeSetting = session.Query<LogisticsRepackTypeSettingInfo>()
            .Where(e => e.Type == repackType)
            .Take(1)
            .SingleOrDefault();

        string repackTypeTitle = repackType.Name;
        if (repackTypeSetting != null) {
            repackTypeTitle = repackTypeSetting.Title;
        }

        // 订单图片
        var expressPhotoList = session.Query<ExpressPhotoInfo>()
            .Where(e => trackingNoList.Contains(e.TrackingNo) || e.Order!.Id == orderId)
            .ToList();

        foreach (var p in expressPhotoList) {
            p.BigImageFullUrlCache = expressPhotoFileService.GetBigImageFullUrlForSite(p, OssSiteLocType.China);
            p.SmallImageFullUrlCache = expressPhotoFileService.GetSmallImageFullUrlForSite(p, OssSiteLocType.China);
        }

        bool hasPassResCancelOrder = passResService.UserHasPassRes(loginUserId, PassRes.LogisticsOrder.OrderSetCancel);
        bool statusCanCancelOrder = LogisticsOrderStatus.GetAdminCanCancelStatusList().Contains(order.OrderStatus);

        bool hasPassResCompleteOrder = passResService.UserHasPassRes(loginUserId, PassRes.LogisticsOrder.OrderSetComplete);
        bool statusCanCompleteOrder = LogisticsOrderStatus.GetAdminCanCompleteStatusList().Contains(order.OrderStatus);

        var model = new LogisticsOrderDetailIndexModel {
            MenuType = menuType,
            Order = order,
            RepackTypeTitle = repackTypeTitle,
            ReceiveAddress = receiveAddress,
            // SendAddress = sendAddress,
            ExpressList = expressList,
            ExpressSizeWeightList = expressSizeWeightList,
            ExpressTotalVolume = expressSizeWeightService.GetTotalVolume(expressSizeWeightList),
            ExpressTotalVolumeWeight = expressSizeWeightService.GetTotalVolumeWeight(expressSizeWeightList),
            ExpressTotalActualWeight = expressSizeWeightService.GetTotalActualWeight(expressSizeWeightList),
            ExpressTotalPackageCount = expressSizeWeightService.GetTotalPackageCount(expressSizeWeightList),
            OrderFirstModeList = orderFirstModeList,
            OrderPackBoxList = orderPackBoxList,
            LogisticsOrderActionList = orderActionList,
            PhotoList = expressPhotoList,
            VolumeWeightDenominator = logisticsModeSettingService.GetSetting().VolumeWeightDenominator,
            CanCancelOrder = hasPassResCancelOrder && statusCanCancelOrder,
            CanCompleteOrder = hasPassResCompleteOrder && statusCanCompleteOrder,
        };

        return View("LogisticsOrderDetailIndex", model);
    }

    [AdminAuth(PassRes.LogisticsOrder.OrderSetCancel)]
    [HttpPost]
    [Route("/admin/logistics/order/detail/cancel/order")]
    [Transactional]
    public IActionResult CancelOrder(IdParam param) {
        ValidationUtil.Validate<IdParamValidator, IdParam>(param);

        int orderId = param.Id;

        int loginUserId = loginUserService.GetLoginUserId();

        logisticsOrderService.CancelOrder(new LogisticsOrderCancelParam {
            OrderId = orderId,
            Backend = true,
            UserId = loginUserId,
        });

        return Json(new JsonResultData {
            Success = true,
            Message = "作废订单成功！",
        });
    }

    [AdminAuth(PassRes.LogisticsOrder.OrderSetComplete)]
    [HttpPost]
    [Route("/admin/logistics/order/detail/complete/order")]
    [Transactional]
    public IActionResult CompleteOrder(IdParam param) {
        ValidationUtil.Validate<IdParamValidator, IdParam>(param);

        int orderId = param.Id;

        int loginUserId = loginUserService.GetLoginUserId();

        logisticsOrderService.CompleteOrder(new LogisticsOrderCompleteParam {
            OrderId = orderId,
            UserId = loginUserId,
        });

        return Json(new JsonResultData {
            Success = true,
            Message = "完成订单成功！",
        });
    }
}
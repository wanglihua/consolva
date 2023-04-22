using Microsoft.AspNetCore.Mvc;
using NHibernate;
using shunda.Client.Areas.Order.Models.OrderLogisticsTrack;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.LogisticsTrack;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderLogisticsTrackController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public OrderLogisticsTrackController(
        ISession session,
        LoginCustomerService loginCustomerService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/order/logistics/track")]
    public IActionResult Index([FromBody] OrderLogisticsTrackParam param) {
        ValidationUtil.Validate<OrderLogisticsTrackParamValidator, OrderLogisticsTrackParam>(param);

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var loginCustomer = loginCustomerService.GetLoginCustomer();

        int orderFirstModeId = param.OrderFirstModeId;
        var orderFirstMode = session.Get<LogisticsOrderFirstModeInfo>(orderFirstModeId);
        if (orderFirstMode == null) {
            throw new ValidationException($"指定的订单物流#{orderFirstModeId}不存在！");
        }

        NHibernateUtil.Initialize(orderFirstMode.Order);
        NHibernateUtil.Initialize(orderFirstMode.FirstMode);

        var order = orderFirstMode.Order;
        var firstMode = orderFirstMode.FirstMode;

        if (order.Customer.Id != loginCustomerId) {
            throw new ValidationException($"订单#{order.Id}不属于当前登录客户#{loginCustomer.CheckinCode}！");
        }

        string trackingNo = orderFirstMode.TrackingNo;
        if (string.IsNullOrWhiteSpace(trackingNo)) {
            throw new ValidationException($"指定的订单物流#{orderFirstModeId}未导入国际单号！");
        }

        var trackNumber = session.Query<SeventeenTrackNumberInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();
        if (trackNumber == null) {
            throw new ValidationException($"指定的国际单号#{trackingNo}未注册！");
        }

        int trackNumberId = trackNumber.Id;
        IList<SeventeenTrackNumberEventInfo> eventList = session
            .Query<SeventeenTrackNumberEventInfo>()
            .Where(e => e.TrackNumber.Id == trackNumberId)
            .ToList();

        eventList = eventList.OrderBy(e => e.EventTime).ThenBy(e => e.Id).ToList();
        var eventItemList = eventList.Select(e => new OrderLogisticsTrackEventItem {
            EventTime = e.EventTime.ToString("yyyy-MM-dd HH:mm"),
            EventDesc = e.EventDesc,
        }).ToList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderLogisticsTrackModel {
                OrderCode = order.Code,
                FirstModeName = firstMode.Name,
                GoodsType = orderFirstMode.GoodsType.Name,
                TrackingNo = trackingNo,
                CarrierName = trackNumber.CarrierName,
                EventList = eventItemList,
            },
        });
    }
}
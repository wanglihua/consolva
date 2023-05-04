using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHibernate.Linq;
using NLog;
using shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackWebHook;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.LogisticsTrack;
using shunda.Service.LogisticsOrder;
using shunda.Service.logisticsTrack.SeventeenTrack.Carrier;
using shunda.Service.Message.CustomerMessage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsTrack.Controllers;

[LogisticsTrackArea]
[NoLoginRequired]
public class SeventeenTrackWebHookController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly SeventeenTrackCarrierService seventeenTrackCarrierService;
    private readonly CustomerMessageInternationalExpressStateUpdateService customerMessageInternationalExpressStateUpdateService;
    private readonly CustomerMessageOrderDeliveredService customerMessageOrderDeliveredService;
    private readonly LogisticsOrderService logisticsOrderService;
    private readonly LogisticsOrderPackBoxHistoryService logisticsOrderPackBoxHistoryService;

    public SeventeenTrackWebHookController(
        ISession session,
        SeventeenTrackCarrierService seventeenTrackCarrierService,
        CustomerMessageInternationalExpressStateUpdateService customerMessageInternationalExpressStateUpdateService,
        CustomerMessageOrderDeliveredService customerMessageOrderDeliveredService,
        LogisticsOrderService logisticsOrderService,
        LogisticsOrderPackBoxHistoryService logisticsOrderPackBoxHistoryService
    ) {
        this.session = session;
        this.seventeenTrackCarrierService = seventeenTrackCarrierService;
        this.customerMessageInternationalExpressStateUpdateService = customerMessageInternationalExpressStateUpdateService;
        this.customerMessageOrderDeliveredService = customerMessageOrderDeliveredService;
        this.logisticsOrderService = logisticsOrderService;
        this.logisticsOrderPackBoxHistoryService = logisticsOrderPackBoxHistoryService;
    }

    [HttpPost]
    [Route("/admin/seventeen/track/web/hook/notify")]
    // 这里不加事务也行的
    public IActionResult Notify() {
        string requestBody = string.Empty;

        try {
            if (Request.Headers.ContainsKey("sign") == false || Request.Headers["sign"].Count == 0) {
                return StatusCode((int)HttpStatusCode.NotAcceptable);
            }

            string sign = Request.Headers["sign"][0];
            // log.Error("sign:");
            // log.Error(sign);

            StreamReader streamReader = new StreamReader(Request.Body);
            requestBody = streamReader.ReadToEndAsync().GetAwaiter().GetResult();

            // 过段时间后，把这个去掉
            // log.Error("requestBody:");
            // log.Error(requestBody);

            // 签名验证
            if (sign != SHA256Util.Encrypt($"{requestBody}/{LogisticsTrackConfig.SeventeenTrackToken}")) {
                return StatusCode((int)HttpStatusCode.NotAcceptable);
            }

            var model = JsonConvert.DeserializeObject<SeventeenTrackWebHookRequestModel>(requestBody);
            if (model == null) {
                log.Error("model == null");
                log.Error(requestBody);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            if (model.Event == "TRACKING_STOPPED") {
                log.Error(requestBody); // 暂时记录一下
                return StatusCode((int)HttpStatusCode.OK);
            }

            // 就两种情况 TRACKING_UPDATED 和 TRACKING_STOPPED
            // if (model.Event == "TRACKING_UPDATED") 

            string trackingNo = model.Data.Number;
            int carrierKey = model.Data.Carrier;

            // 查看运输商是否本地存在
            var carrier = session.Query<SeventeenTrackCarrierInfo>()
                .Where(e => e.CarrierKey == carrierKey)
                .Take(1)
                .SingleOrDefault();
            if (carrier == null) {
                seventeenTrackCarrierService.SyncData();

                carrier = session.Query<SeventeenTrackCarrierInfo>()
                    .Where(e => e.CarrierKey == carrierKey)
                    .Take(1)
                    .SingleOrDefault();
                if (carrier == null) {
                    log.Error($"17TRACK运输商#{carrierKey}不存在！");
                    log.Error(requestBody);
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
            }

            // 查看 tracking no 是否本地存在，并更新 tracking no 状态和子状态
            var trackNumber = session.Query<SeventeenTrackNumberInfo>()
                .Where(e => e.TrackingNo == trackingNo)
                .Take(1)
                .SingleOrDefault();
            if (trackNumber == null) {
                trackNumber = new SeventeenTrackNumberInfo {
                    TrackingNo = trackingNo,
                    CarrierKey = carrierKey,
                    CarrierName = carrier.CarrierName,
                    Origin = SeventeenTrackNumberOrigin.None,
                    ErrorCode = 0,
                    ErrorMessage = string.Empty,
                    Status = SeventeenTrackNumberStatus.Accepted,
                    TrackStatus = model.Data.TrackInfo.LatestStatus.Status,
                    TrackSubStatus = model.Data.TrackInfo.LatestStatus.SubStatus,
                };

                session.Save(trackNumber);
            } else {
                trackNumber.TrackStatus = model.Data.TrackInfo.LatestStatus.Status;
                trackNumber.TrackSubStatus = model.Data.TrackInfo.LatestStatus.SubStatus;

                session.Update(trackNumber);
            }

            session.Flush();

            // 更新 tracking number events
            int trackNumberId = trackNumber.Id;
            var trackNumberEventList = session.Query<SeventeenTrackNumberEventInfo>()
                .Where(e => e.TrackNumber.Id == trackNumberId)
                .ToList();
            // var trackNumberEventDict = trackNumberEventList.ToDictionary(e => e.EventTime, e => e); 他同一个时间会有多个事件，这里只假设了一个，会报错

            var provider = model.Data.TrackInfo.Tracking.ProviderList
                .Where(e => e.Provider.Key == carrierKey)
                .Take(1)
                .SingleOrDefault();
            if (provider == null) {
                log.Error("provider == null");
                log.Error(requestBody);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            SeventeenTrackNumberEventInfo? latestNumberEvent = null;

            var eventList = provider.EventList.Reverse(); // 这里原来是按时间倒序排列的

            // 假设按时间正序循环
            foreach (var evt in eventList) {
                if (DateTime.TryParse(evt.TimeIso, out DateTime eventTime) == false) {
                    log.Error($"{evt.TimeIso}不是正确的时间格式！");
                    log.Error(requestBody);
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }

                string eventDesc = evt.Description ?? string.Empty;
                string stage = evt.Stage ?? string.Empty;

                var trackNumberEventInDb = trackNumberEventList
                    .Where(e => e.EventTime == eventTime && e.EventDesc == eventDesc && e.Stage == stage)
                    .Take(1)
                    .SingleOrDefault();

                if (trackNumberEventInDb == null) {
                    var trackNumberEvent = new SeventeenTrackNumberEventInfo {
                        TrackNumber = trackNumber,
                        EventTime = eventTime,
                        EventDesc = eventDesc,
                        Stage = stage,
                    };

                    session.Save(trackNumberEvent);

                    latestNumberEvent = trackNumberEvent;
                }
            }

            session.Flush();

            // 可能的发送消息
            if (latestNumberEvent != null) {
                var orderFirstMode = session.Query<LogisticsOrderFirstModeInfo>()
                    .Where(e => e.TrackingNo == trackingNo)
                    .Take(1)
                    .SingleOrDefault();

                if (orderFirstMode != null) {
                    var customer = orderFirstMode.Order.Customer;
                    var order = orderFirstMode.Order;
                    string trackingContent = latestNumberEvent.EventDesc;

                    // 快递单动态有更新
                    customerMessageInternationalExpressStateUpdateService.SendMessage(customer, order, trackingNo, trackingContent);

                    // Delivered
                    if (latestNumberEvent.Stage == "Delivered") {
                        int orderId = order.Id;
                        var orderPackBoxList = session.Query<LogisticsOrderPackBoxInfo>()
                            .Fetch(e => e.OrderPack)
                            .Where(e => e.Order.Id == orderId)
                            .ToList();

                        int orderFirstModeId = orderFirstMode.Id;
                        var orderFirstModePackBoxList = orderPackBoxList
                            .Where(e => e.OrderPack.OrderFirstMode.Id == orderFirstModeId)
                            .ToList();

                        foreach (var packBox in orderFirstModePackBoxList) {
                            if (packBox.Status != LogisticsOrderPackBoxStatus.Deliver) {
                                var packBoxStatusBefore = packBox.Status;

                                packBox.Status = LogisticsOrderPackBoxStatus.Deliver;
                                session.Update(packBox);

                                var packBoxStatusAfter = packBox.Status;

                                logisticsOrderPackBoxHistoryService.AddHistory(packBox, LogisticsOrderPackBoxHistoryType.Deliver, packBoxStatusBefore, packBoxStatusAfter);
                            }
                        }

                        int notDeliverPackBoxCount = orderPackBoxList.Where(e => e.Status != LogisticsOrderPackBoxStatus.Deliver).Count();

                        if (notDeliverPackBoxCount == 0) {
                            customerMessageOrderDeliveredService.SendMessage(customer, order);

                            // 结束订单
                            logisticsOrderService.CompleteOrder(new LogisticsOrderCompleteParam {
                                OrderId = orderId,
                                UserId = 0,
                            });
                        }
                    }
                }
            }

            session.Flush();
        }
        catch (Exception exp) {
            log.Error(exp);
            log.Error(requestBody);
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        return StatusCode((int)HttpStatusCode.OK);
    }
}
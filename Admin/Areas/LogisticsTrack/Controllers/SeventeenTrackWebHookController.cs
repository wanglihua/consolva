using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackWebHook;
using shunda.Core.Config;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.LogisticsTrack;
using shunda.Service.logisticsTrack.SeventeenTrack.Carrier;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsTrack.Controllers;

[LogisticsTrackArea]
[NoLoginRequired]
public class SeventeenTrackWebHookController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly SeventeenTrackCarrierService seventeenTrackCarrierService;

    public SeventeenTrackWebHookController(
        ISession session,
        SeventeenTrackCarrierService seventeenTrackCarrierService
    ) {
        this.session = session;
        this.seventeenTrackCarrierService = seventeenTrackCarrierService;
    }

    [HttpPost]
    [Route("/admin/seventeen/track/web/hook/notify")]
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

            var eventList = provider.EventList.Reverse(); // 这里是按时间倒序排列的

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
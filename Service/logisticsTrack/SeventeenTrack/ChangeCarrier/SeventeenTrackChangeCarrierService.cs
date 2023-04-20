using NHibernate;
using NLog;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Domain.LogisticsTrack;

namespace shunda.Service.logisticsTrack.SeventeenTrack.ChangeCarrier;

/// <summary>
/// 17track更改注册商
/// </summary>
[Service(ServiceType.Scoped)]
public class SeventeenTrackChangeCarrierService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private static readonly int MAX_TRACKING_NO_COUNT_ONCE = 40;

    private readonly ISession session;

    public SeventeenTrackChangeCarrierService(ISession session) {
        this.session = session;
    }


    public void ChangeCarrier(IList<SeventeenTrackChangeCarrierRequestItem> requestDataItemList) {
        if (requestDataItemList.Count == 0) {
            throw new ValidationException("更改运输商单号列表不能为空！");
        }

        List<SeventeenTrackChangeCarrierRequestItem> requestDataItemListRemind = requestDataItemList.ToList();

        while (requestDataItemListRemind.Count != 0) {
            List<SeventeenTrackChangeCarrierRequestItem> requestDataItemListOnce = requestDataItemListRemind.Take(MAX_TRACKING_NO_COUNT_ONCE).ToList();
            requestDataItemListRemind = requestDataItemListRemind.Skip(MAX_TRACKING_NO_COUNT_ONCE).ToList();

            ChangeCarrierInternal(requestDataItemListOnce);
        }
    }

    private void ChangeCarrierInternal(IList<SeventeenTrackChangeCarrierRequestItem> requestDataItemList) {
        if (requestDataItemList.Count == 0) {
            return;
        }

        if (requestDataItemList.Count > MAX_TRACKING_NO_COUNT_ONCE) {
            throw new ValidationException("一次更改运输商单号数量不能超过40，请分批次更改！");
        }

        // https://api.17track.net/track/v2/changecarrier

        using var client = new RestClient(
            new RestClientOptions {
                BaseUrl = new Uri("https://api.17track.net"),
            },
            configureSerialization: c => c.UseNewtonsoftJson()
        );

        var request = new RestRequest("/track/v2/changecarrier");
        request.AddHeader("17token", LogisticsTrackConfig.SeventeenTrackToken);
        request.AddHeader("Content-Type", "application/json");

        request.AddJsonBody(requestDataItemList);

        request.OnBeforeDeserialization = response => {
            string contentType = response.ContentType ?? string.Empty;
            if (contentType != "application/json") {
                string content = response.Content ?? string.Empty;
                log.Error($"contentType: {contentType}");
                log.Error($"content: {content}");
                throw new ValidationException(content);
            }
        };

        SevenTrackResponse<SeventeenTrackChangeCarrierResponseData>? responseData = null;

        try {
            responseData = client.Post<SevenTrackResponse<SeventeenTrackChangeCarrierResponseData>>(request);
        }
        catch (Exception exp) {
            log.Error(exp);
            throw new Exception($"更改快递单号17track运输商数据出现异常: {exp.Message}");
        }

        if (responseData == null
            || responseData.Code != 0
            || (responseData.Data.ErrorList != null && responseData.Data.ErrorList.Count != 0)) {
            string errorMessage = "更改快递单号17track运输商失败！";
            if ((responseData?.Data.ErrorList?.Count ?? 0) != 0) {
                errorMessage = errorMessage + string.Join(",", responseData?.Data.ErrorList?.Select(e => e.Message).ToList() ?? new List<string>());
            }

            throw new ValidationException(errorMessage);
        }

        // 将返回结果放入数据库
        var trackingNoList = requestDataItemList.Select(e => e.Number).ToList();
        var seventeenTrackNumberList = session.Query<SeventeenTrackNumberInfo>()
            .Where(e => trackingNoList.Contains(e.TrackingNo))
            .ToList();

        var acceptedList = responseData.Data.AcceptedList;
        if (acceptedList != null && acceptedList.Count != 0) {
            var carrierKeyList = acceptedList.Select(e => e.Carrier).Distinct().ToList();
            var carrierList = session.Query<SeventeenTrackCarrierInfo>()
                .Where(e => carrierKeyList.Contains(e.CarrierKey))
                .ToList();

            foreach (var accepted in acceptedList) {
                int carrierKey = accepted.Carrier;
                var carrier = carrierList
                    .Where(e => e.CarrierKey == carrierKey)
                    .Take(1)
                    .SingleOrDefault();

                string trackingNo = accepted.Number;

                var seventeenTrackNumber = seventeenTrackNumberList
                    .Where(e => e.TrackingNo == trackingNo)
                    .Take(1)
                    .SingleOrDefault();

                if (seventeenTrackNumber == null) {
                    seventeenTrackNumber = new SeventeenTrackNumberInfo {
                        TrackingNo = trackingNo,
                    };

                    seventeenTrackNumberList.Add(seventeenTrackNumber); // 要加进去，否则当前列表有重复的，就会报错了                    
                }

                seventeenTrackNumber.TrackingNo = trackingNo;
                seventeenTrackNumber.CarrierKey = carrierKey;
                seventeenTrackNumber.CarrierName = carrier?.CarrierName ?? "未知运输商";
                seventeenTrackNumber.Origin = SeventeenTrackNumberOrigin.Accurate;
                seventeenTrackNumber.ErrorCode = 0;
                seventeenTrackNumber.ErrorMessage = string.Empty;
                seventeenTrackNumber.Status = SeventeenTrackNumberStatus.Accepted;

                session.SaveOrUpdate(seventeenTrackNumber);
            }

            session.Flush();
        }

        var rejectedList = responseData.Data.RejectedList;
        if (rejectedList != null && rejectedList.Count != 0) {
            foreach (var rejected in rejectedList) {
                string trackingNo = rejected.Number;

                var seventeenTrackNumber = seventeenTrackNumberList
                    .Where(e => e.TrackingNo == trackingNo)
                    .Take(1)
                    .SingleOrDefault();

                // 原来数据库中没记录的就先不管了
                if (seventeenTrackNumber != null) {
                    seventeenTrackNumber.ErrorMessage = rejected.Error.Message;
                    seventeenTrackNumber.Status = SeventeenTrackNumberStatus.Rejected;

                    session.Update(seventeenTrackNumber);
                }
            }
        }

        session.Flush();
    }
}
using NHibernate;
using NLog;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Domain.LogisticsTrack;

namespace shunda.Service.logisticsTrack.SeventeenTrack.Register;

/// <summary>
/// 17track单号注册
/// </summary>
[Service(ServiceType.Scoped)]
public class SeventeenTrackRegisterService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private static readonly int MAX_TRACKING_NO_COUNT_ONCE = 40;

    private readonly ISession session;

    public SeventeenTrackRegisterService(ISession session) {
        this.session = session;
    }

    public void Register(List<SevenTrackRegisterRequestItem> requestDataItemList) {
        if (requestDataItemList.Count == 0) {
            throw new ValidationException("注册单号列表不能为空！");
        }

        if (requestDataItemList.Count > MAX_TRACKING_NO_COUNT_ONCE) {
            throw new ValidationException("一次注册单号数量不能超过40，请分批次注册！");
        }

        List<SevenTrackRegisterRequestItem> requestDataItemListRemind = requestDataItemList;

        while (requestDataItemListRemind.Count != 0) {
            List<SevenTrackRegisterRequestItem> requestDataItemListOnce = requestDataItemListRemind.Take(MAX_TRACKING_NO_COUNT_ONCE).ToList();
            requestDataItemListRemind = requestDataItemListRemind.Skip(MAX_TRACKING_NO_COUNT_ONCE).Take(MAX_TRACKING_NO_COUNT_ONCE).ToList();

            RegisterInternal(requestDataItemListOnce);
        }
    }

    private void RegisterInternal(List<SevenTrackRegisterRequestItem> requestDataItemList) {
        if (requestDataItemList.Count == 0) {
            throw new ValidationException("注册单号列表不能为空！");
        }

        if (requestDataItemList.Count > MAX_TRACKING_NO_COUNT_ONCE) {
            throw new ValidationException("一次注册单号数量不能超过40，请分批次注册！");
        }

        // https://api.17track.net/track/v2/register

        using var client = new RestClient(
            new RestClientOptions {
                BaseUrl = new Uri("https://api.17track.net"),
            },
            configureSerialization: c => c.UseNewtonsoftJson()
        );

        var request = new RestRequest("/track/v2/register");
        request.AddHeader("17token", LogisticsTrackConfig.SeventeenTrackToken);
        request.AddHeader("Content-Type", "application/json");

        request.AddJsonBody(requestDataItemList);

        request.OnBeforeDeserialization = response => {
            log.Error(response.ContentType ?? string.Empty);
            log.Error(response.Content ?? string.Empty);
        };

        SevenTrackResponse<SevenTrackRegisterResponseData>? responseData = null;

        try {
            responseData = client.Post<SevenTrackResponse<SevenTrackRegisterResponseData>>(request);
        }
        catch (Exception exp) {
            log.Error(exp);
            throw new Exception($"注册17track快递单数据出现异常: {exp.Message}");
        }

        if (responseData == null
            || responseData.Code != 0
            || (responseData.Data.ErrorList != null && responseData.Data.ErrorList.Count != 0)) {
            string errorMessage = "注册17track快递单失败！";
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
                }

                seventeenTrackNumber.TrackingNo = trackingNo;
                seventeenTrackNumber.CarrierKey = carrierKey;
                seventeenTrackNumber.CarrierName = carrier?.CarrierName ?? "未知运输商";
                seventeenTrackNumber.Origin = SeventeenTrackNumberOrigin.GetById(accepted.Origin);
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

                if (seventeenTrackNumber == null) {
                    seventeenTrackNumber = new SeventeenTrackNumberInfo {
                        TrackingNo = trackingNo,
                        CarrierKey = 0,
                        CarrierName = string.Empty,
                        Origin = SeventeenTrackNumberOrigin.None,
                        ErrorCode = rejected.Error.Code,
                        ErrorMessage = rejected.Error.Message,
                        Status = SeventeenTrackNumberStatus.Rejected,
                    };

                    session.Save(seventeenTrackNumber);
                } else {
                    // 之前被拒绝的才有可能要更新最新的错误相关信息等，其他的状态不需要更新。比如重复注册了，更不用更新。
                    if (seventeenTrackNumber.Status == SeventeenTrackNumberStatus.Rejected) {
                        // seventeenTrackNumber.TrackingNo = trackingNo;
                        seventeenTrackNumber.CarrierKey = 0;
                        seventeenTrackNumber.CarrierName = string.Empty;
                        seventeenTrackNumber.Origin = SeventeenTrackNumberOrigin.None;
                        seventeenTrackNumber.ErrorCode = rejected.Error.Code;
                        seventeenTrackNumber.ErrorMessage = rejected.Error.Message;
                        seventeenTrackNumber.Status = SeventeenTrackNumberStatus.Rejected;

                        session.Update(seventeenTrackNumber);
                    }
                }
            }

            session.Flush();
        }
    }
}
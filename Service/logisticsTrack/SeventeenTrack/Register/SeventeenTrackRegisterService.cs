using NHibernate;
using NLog;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using shunda.Core;
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
            requestDataItemListRemind = requestDataItemListRemind.Take(MAX_TRACKING_NO_COUNT_ONCE).ToList();

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

                var seventeenTrackNumber = new SeventeenTrackNumberInfo {
                    TrackingNo = accepted.Number,
                    CarrierKey = carrierKey,
                    CarrierName = carrier?.CarrierName ?? "未知运输商",
                    Origin = SeventeenTrackNumberOrigin.GetById(accepted.Origin),
                    ErrorCode = 0,
                    ErrorMessage = string.Empty,
                    Status = SeventeenTrackNumberStatus.Accepted,
                };

                session.Save(seventeenTrackNumber);
            }

            session.Flush();
        }

        var rejectedList = responseData.Data.RejectedList;
        if (rejectedList != null && rejectedList.Count != 0) {
            foreach (var rejected in rejectedList) {
                var seventeenTrackNumber = new SeventeenTrackNumberInfo {
                    TrackingNo = rejected.Number,
                    CarrierKey = 0,
                    CarrierName = string.Empty,
                    Origin = SeventeenTrackNumberOrigin.None,
                    ErrorCode = rejected.Error.Code,
                    ErrorMessage = rejected.Error.Message,
                    Status = SeventeenTrackNumberStatus.Rejected,
                };
                
                session.Save(seventeenTrackNumber);
            }
            
            session.Flush();
        }
    }
}
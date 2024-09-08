using NHibernate;
using NLog;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.LogisticsTrack;

namespace shunda.Service.logisticsTrack.SeventeenTrack.Carrier;

/// <summary>
/// 17TRACK运输商 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class SeventeenTrackCarrierService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public SeventeenTrackCarrierService(ISession session)
    {
        this.session = session;
    }

    public void SyncData()
    {
        using var client = new RestClient(
            new RestClientOptions { BaseUrl = new Uri("https://res.17track.net"), },
            configureSerialization: c => c.UseNewtonsoftJson()
        );

        var request = new RestRequest("/asset/carrier/info/apicarrier.all.json");

        // var response = client.Get(request);
        // response.StatusCode;

        IList<SeventeenTrackCarrierItem>? carrierList = null;
        try
        {
            carrierList = client.Get<IList<SeventeenTrackCarrierItem>>(request);
        }
        catch (Exception exp)
        {
            log.Error(exp);
            throw new Exception($"下载17TRACK运输商数据出现异常: {exp.Message}");
        }

        if (carrierList == null)
        {
            throw new ValidationException("下载17TRACK运输商数据失败！");
        }

        var carrierListInDb = session.Query<SeventeenTrackCarrierInfo>().ToList();
        var carrierDict = carrierListInDb.ToDictionary(e => e.CarrierKey, e => e);

        foreach (var c in carrierList)
        {
            SeventeenTrackCarrierInfo carrier;
            if (carrierDict.ContainsKey(c.CarrierKey))
            {
                carrier = carrierDict[c.CarrierKey];
            }
            else
            {
                carrier = new SeventeenTrackCarrierInfo();
                carrier.CarrierKey = c.CarrierKey;
            }

            carrier.CountryCode = c.CountryCode;
            carrier.CountryIso = c.CountryIso;
            carrier.Email = c.Email;
            carrier.Tel = c.Tel;
            carrier.Url = c.Url;
            carrier.CarrierName = c.CarrierName;
            carrier.NameZhCn = c.NameZhCn;
            carrier.NameZhHk = c.NameZhHk;
            carrier.CarrierGroup = c.CarrierGroup;

            session.SaveOrUpdate(carrier);
        }

        session.Flush();
    }

    /// <summary>
    /// 根据快递单号特征确定运输商
    /// </summary>
    /// <param name="trackingNo"></param>
    /// <returns></returns>
    public int? GetKeyByTrackingNo(string trackingNo)
    {
        if (string.IsNullOrWhiteSpace(trackingNo))
        {
            return null;
        }

        trackingNo = trackingNo.Trim().ToUpper();

        if (trackingNo.StartsWith("ACWL"))
        {
            // ac logistics
            return 190512;
        }

        if (trackingNo.StartsWith("33X") || trackingNo.StartsWith("3698") || trackingNo.StartsWith("36HFR"))
        {
            //UBI
            return 190112;
        }

        if (trackingNo.StartsWith("34Q"))
        {
            // ASIAFLY
            return 190303;
        }

        if (trackingNo.StartsWith("3676E") || trackingNo.StartsWith("3674"))
        {
            // Australia Post
            return 1151;
        }

        // 36CCP300125701000935100  36CC开头的运输商帮设置成澳邮
        if (trackingNo.StartsWith("36CC"))
        {
            // Australia Post
            return 1151;
        }

        /*
            3676F406558201000931509 ----- 3676
            36HKW300701301000935100 ----  36H
         */

        if (trackingNo.StartsWith("3676") || trackingNo.StartsWith("36H"))
        {
            // Australia Post
            return 1151;
        }


        if ((trackingNo.StartsWith("397") || trackingNo.StartsWith("398")) && trackingNo.Length == 12)
        {
            // FedEx
            return 100003;
        }

        if (trackingNo.StartsWith("778"))
        {
            // FedEx
            return 100003;
        }

        if (trackingNo.StartsWith("2716"))
        {
            // FedEx
            return 100003;
        }

        if (trackingNo.StartsWith("272"))
        {
            // FedEx
            return 100003;
        }

        // if (trackingNo.StartsWith("780")) {
        if (trackingNo.StartsWith("78"))
        {
            // FedEx
            return 100003;
        }

        if (trackingNo.StartsWith("88"))
        {
            // Toll
            return 100009;
        }

        if (trackingNo.StartsWith("816"))
        {
            // Lenton Group (DPD)
            return 100886;
        }

        return null;
    }
}
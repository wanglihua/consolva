using NHibernate;
using NLog;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.LogisticsTrack;

namespace shunda.Service.logisticsTrack.Carrier;

/// <summary>
/// 17track运输商 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class SeventeenTrackCarrierService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public SeventeenTrackCarrierService(ISession session) {
        this.session = session;
    }

    public void SyncData() {
        // https://carrierlist.17track.net
        var client = new RestClient(
            new RestClientOptions {
                BaseUrl = new Uri("https://res.17track.net/asset/carrier/info/apicarrier.all.json"),
            },
            configureSerialization: c => c.UseNewtonsoftJson()
        );

        var request = new RestRequest("");

        // var response = client.Get(request);
        // response.StatusCode;

        IList<SeventeenTrackCarrierItem>? carrierList = null;
        try {
            carrierList = client.Get<IList<SeventeenTrackCarrierItem>>(request);
        }
        catch (Exception exp) {
            log.Error(exp);
            throw new Exception($"下载17track运输商数据出现异常: {exp.Message}");
        }

        if (carrierList == null) {
            throw new ValidationException("下载17track运输商数据失败！");
        }

        var carrierListInDb = session.Query<SeventeenTrackCarrierInfo>().ToList();
        var carrierDict = carrierListInDb.ToDictionary(e => e.CarrierKey, e => e);

        foreach (var c in carrierList) {
            SeventeenTrackCarrierInfo carrier;
            if (carrierDict.ContainsKey(c.CarrierKey)) {
                carrier = carrierDict[c.CarrierKey];
            } else {
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
}
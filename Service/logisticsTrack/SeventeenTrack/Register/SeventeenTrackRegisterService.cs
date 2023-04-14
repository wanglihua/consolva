using NHibernate;
using NLog;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using shunda.Core;
using shunda.Core.Service;

namespace shunda.Service.logisticsTrack.SeventeenTrack.Register;

/// <summary>
/// 17track单号注册
/// </summary>
[Service(ServiceType.Scoped)]
public class SeventeenTrackRegisterService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public SeventeenTrackRegisterService(ISession session) {
        this.session = session;
    }

    public void Register(SevenTrackRegisterRequestData requestData) {
        if (requestData.Count == 0) {
            throw new ValidationException("注册单号列表不能为空！");
        }

        // https://api.17track.net/track/v2/register

        using var client = new RestClient(
            new RestClientOptions {
                BaseUrl = new Uri("https://api.17track.net"),
            },
            configureSerialization: c => c.UseNewtonsoftJson()
        );

        var request = new RestRequest("/track/v2/register");

        try {
            var responseData = client.Post<IList<SevenTrackResponse<SevenTrackRegisterResponseData>>>(request);
        }
        catch (Exception exp) {
            log.Error(exp);
            throw new Exception($"注册17track快递单数据出现异常: {exp.Message}");
        }
    }
}
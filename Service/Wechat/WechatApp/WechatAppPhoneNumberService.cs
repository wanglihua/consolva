using Newtonsoft.Json;
using NLog;
using shunda.Core.Service;

namespace shunda.Service.Wechat.WechatApp;

/// <summary>
/// phonenumber.getPhoneNumber
/// https://developers.weixin.qq.com/miniprogram/dev/api-backend/open-api/phonenumber/phonenumber.getPhoneNumber.html
/// </summary>
[Service(ServiceType.Scoped)]
public class WechatAppPhoneNumberService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly HttpClient httpClient;
    private readonly WechatAppAccessTokenService wechatAppAccessTokenService;

    public WechatAppPhoneNumberService(HttpClient httpClient, WechatAppAccessTokenService wechatAppAccessTokenService)
    {
        this.httpClient = httpClient;
        this.wechatAppAccessTokenService = wechatAppAccessTokenService;
    }

    public string GetPhoneNumber(string code)
    {
        string accessToken = wechatAppAccessTokenService.GetAccessToken();

        // POST https://api.weixin.qq.com/wxa/business/getuserphonenumber?access_token=ACCESS_TOKEN
        IList<string> urlPartList = new List<string>() {
            "https://api.weixin.qq.com/wxa/business/getuserphonenumber",
            $"?access_token={accessToken}",
            $"&code={code}",
        };

        // 现在是 url 和 form 中都传上了这个值，实际只要一方有这个值就可以了?
        var formValues = new Dictionary<string, string>() {
            { "access_token", accessToken },
            { "code", code },
        };

        var taskPost = httpClient.PostAsync(string.Join("", urlPartList), new FormUrlEncodedContent(formValues));
        taskPost.Wait();

        var responseMessge = taskPost.Result;
        var taskRead = responseMessge.Content.ReadAsStringAsync();
        taskRead.Wait();

        var result = JsonConvert.DeserializeObject<PhoneNumberResult>(taskRead.Result);

        if (result == null)
        {
            throw new Exception("微信小程序 phonenumber.getPhoneNumber 出错，未能获取返回结果！");
        }

        if (result.ErrorCode != 0)
        {
            throw new Exception($"微信小程序 phonenumber.getPhoneNumber 出错, 错误代码:{result.ErrorCode}, 错误消息:{result.ErrorMessage}");
        }

        log.Trace($"phone number: {result.Phone.PhoneNumber}");
        log.Trace($"pure phone number: {result.Phone.PurePhoneNumber}");
        log.Trace($"country code: {result.Phone.CountryCode}");

        return result.Phone.PhoneNumber;
    }

    /*
     
        {
            "errcode":0,
            "errmsg":"ok",
            "phone_info": {
                "phoneNumber":"xxxxxx",
                "purePhoneNumber": "xxxxxx",
                "countryCode": 86,
                "watermark": {
                    "timestamp": 1637744274,
                    "appid": "xxxx"
                }
            }
        }
     
     */

    public class PhoneNumberResult : WechatJsonResult
    {
        [JsonProperty("phone_info")]
        public PhoneInfo Phone { get; set; } = new PhoneInfo();

        public class PhoneInfo
        {
            [JsonProperty("phoneNumber")]
            public string PhoneNumber { get; set; } = string.Empty;

            [JsonProperty("purePhoneNumber")]
            public string PurePhoneNumber { get; set; } = string.Empty;

            [JsonProperty("countryCode")]
            public int CountryCode { get; set; }

            [JsonProperty("watermark")]
            public WatermarkInfo Watermark { get; set; } = new WatermarkInfo();

            public class WatermarkInfo
            {
                [JsonProperty("timestamp")]
                public int TimeStamp { get; set; }

                [JsonProperty("appid")]
                public string AppId { get; set; } = string.Empty;
            }
        }
    }
}
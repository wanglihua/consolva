using Newtonsoft.Json;
using NLog;
using shunda.Core.Config;
using shunda.Core.Service;

namespace shunda.Service.Wechat.WechatApp;

/// <summary>
/// auth.code2Session
/// 登录凭证校验。通过 wx.login 接口获得临时登录凭证 code 后传到开发者服务器调用此接口完成登录流程。
/// https://developers.weixin.qq.com/miniprogram/dev/api-backend/open-api/login/auth.code2Session.html
/// </summary>
[Service(ServiceType.Scoped)]
public class WechatAppAuthCodeSessionService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly HttpClient httpClient;

    public WechatAppAuthCodeSessionService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public AuthCodeSessionResult GetResult(string code)
    {
        IList<string> urlPartList = new List<string>() {
            "https://api.weixin.qq.com/sns/jscode2session",
            "?grant_type=client_credential",
            $"&appid={WechatAppConfig.AppId}",
            $"&secret={WechatAppConfig.AppSecret}",
            $"&js_code={code}",
        };

        var task = httpClient.GetStringAsync(string.Join("", urlPartList));
        task.Wait();

        var result = JsonConvert.DeserializeObject<AuthCodeSessionResult>(task.Result);

        if (result == null)
        {
            throw new Exception("微信小程序 auth.code2Session 出错，未能获取返回结果！");
        }

        if (result.ErrorCode != 0)
        {
            throw new Exception($"微信小程序 auth.code2Session 出错, 错误代码:{result.ErrorCode}, 错误消息:{result.ErrorMessage}");
        }
        
        log.Trace($"open id: {result.OpenId}");
        log.Trace($"session key: {result.SessionKey}");
        log.Trace($"union id: {result.UnionId}");

        return result;
    }

    public class AuthCodeSessionResult : WechatJsonResult
    {
        [JsonProperty("openid")]
        public string OpenId { get; set; } = string.Empty;

        [JsonProperty("session_key")]
        public string SessionKey { get; set; } = string.Empty;

        [JsonProperty("unionid")]
        public string UnionId { get; set; } = string.Empty;
    }
}
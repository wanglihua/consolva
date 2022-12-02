using Newtonsoft.Json;
using NLog;
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

    private class AuthCodeSessionResult : WechatJsonResult
    {
        [JsonProperty("openid")]
        public string OpenId { get; set; } = string.Empty;

        [JsonProperty("session_key")]
        public string SessionKey { get; set; } = string.Empty;

        [JsonProperty("unionid")]
        public string UnionId { get; set; } = string.Empty;
    }
}
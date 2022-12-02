using Newtonsoft.Json;
using shunda.Core.Config;
using shunda.Core.Service;

namespace shunda.Service.Wechat.WechatApp;

/// <summary>
/// Get Access Token
/// https://developers.weixin.qq.com/miniprogram/dev/OpenApiDoc/mp-access-token/getAccessToken.html
/// </summary>
[Service(ServiceType.Scoped)]
public class WechatAppAccessTokenService
{
    private readonly HttpClient httpClient;

    public WechatAppAccessTokenService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public string GetAccessToken()
    {
        IList<string> urlPartList = new List<string>() {
            "https://api.weixin.qq.com/cgi-bin/token",
            "?grant_type=client_credential",
            $"&appid={WechatAppConfig.AppId}",
            $"&secret={WechatAppConfig.AppSecret}",
        };

        var task = httpClient.GetStringAsync(string.Join("", urlPartList));
        task.Wait();

        AccessTokenResult? accessTokenResult = JsonConvert.DeserializeObject<AccessTokenResult>(task.Result);

        return accessTokenResult?.AccessToken ?? string.Empty;
    }

    private class AccessTokenResult : WechatJsonResult
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// 凭证有效时间，单位：秒。目前是7200秒之内的值。
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; } = 0;
    }
}
using Newtonsoft.Json;
using shunda.Core.Service;

namespace shunda.Service.Wechat.WechatApp;

/// <summary>
/// Get Access Token
/// https://developers.weixin.qq.com/miniprogram/dev/OpenApiDoc/mp-access-token/getAccessToken.html
/// </summary>
[Service(ServiceType.Scoped)]
public class WechatAppAccessTokenService
{


    public void GetAccessToken()
    {
    }

    private class AccessTokenResult : WechatJsonResult
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; } = 0;
    }    
}
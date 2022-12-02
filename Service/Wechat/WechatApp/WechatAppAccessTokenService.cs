using CSRedis;
using Newtonsoft.Json;
using shunda.Core.Config;
using shunda.Core.Redis;
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

    private string GetAccessTokenRedisKey()
    {
        return $"{RedisClientUtil.RedisKeyPrefix}:weapp_access_token_token";
    }

    private string GetTokenExpireRedisKey()
    {
        return $"{RedisClientUtil.RedisKeyPrefix}:weapp_access_token_expire";
    }

    public string GetAccessToken()
    {
        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();

        DateTime? expire = redisClient.Get<DateTime?>(GetTokenExpireRedisKey());
        string accessToken = redisClient.Get(GetAccessTokenRedisKey());
        // 过期时间缓存存在 并且 过期时间是在当前时间之后 并且 access token 缓存存在
        if (expire != null && expire > DateTime.Now && string.IsNullOrWhiteSpace(accessToken) == false)
        {
            return accessToken;
        }

        IList<string> urlPartList = new List<string>() {
            "https://api.weixin.qq.com/cgi-bin/token",
            "?grant_type=client_credential",
            $"&appid={WechatAppConfig.AppId}",
            $"&secret={WechatAppConfig.AppSecret}",
        };

        var task = httpClient.GetStringAsync(string.Join("", urlPartList));
        task.Wait();

        AccessTokenResult? accessTokenResult = JsonConvert.DeserializeObject<AccessTokenResult>(task.Result);

        if (accessTokenResult == null)
        {
            throw new Exception("微信小程序获取 access token 出错，未能获取返回结果！");
        }

        if (accessTokenResult.ErrorCode != 0)
        {
            throw new Exception($"微信小程序获取 access token 出错, 错误代码:{accessTokenResult.ErrorCode}, 错误消息:{accessTokenResult.ErrorMessage}");
        }

        accessToken = accessTokenResult.AccessToken;

        TimeSpan timeout = TimeSpan.FromSeconds(accessTokenResult.ExpiresIn);
        redisClient.Set(GetAccessTokenRedisKey(), accessToken, timeout);
        // 减去 5 分钟是保证redis中存储的 token 确定有效。
        redisClient.Set(GetTokenExpireRedisKey(), DateTime.Now.AddSeconds(accessTokenResult.ExpiresIn - 5 * 60), timeout);

        return accessToken;
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
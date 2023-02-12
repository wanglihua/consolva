using CSRedis;
using NHibernate;
using NLog;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Entities;
using shunda.Core.Config;
using shunda.Core.Redis;
using shunda.Core.Service;
using shunda.Domain.Wechat;

namespace shunda.Service.Wechat;

[Service(ServiceType.Scoped)]
public class WechatOfficialAccountAccessTokenService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public WechatOfficialAccountAccessTokenService(ISession session) {
        this.session = session;
    }

    private string GetAccessTokenRedisKey() {
        return $"{RedisClientUtil.RedisKeyPrefix}:wechat_official_account_access_token_token";
    }

    private string GetTokenExpireRedisKey() {
        return $"{RedisClientUtil.RedisKeyPrefix}:wechat_official_account_access_token_expire";
    }

    public string GetAccessToken() {
        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();

        DateTime? expire = redisClient.Get<DateTime?>(GetTokenExpireRedisKey());
        string accessToken = redisClient.Get(GetAccessTokenRedisKey());
        // 过期时间缓存存在 并且 过期时间是在当前时间之后 并且 access token 缓存存在
        if (expire != null && expire > DateTime.Now && string.IsNullOrWhiteSpace(accessToken) == false) {
            return accessToken;
        }

        AccessTokenResult result = AccessTokenContainer.GetAccessTokenResult(WechatOfficialAccountConfig.AppId, true);

        if (string.IsNullOrWhiteSpace(result.access_token)) {
            throw new Exception("获取token出错：" + result.errmsg);
        }

        accessToken = result.access_token;
        int expiresIn = result.expires_in;

        log.Trace($"wechat official account access token: {accessToken}");
        log.Trace($"wechat official account token expire: {expiresIn}");

        // 记录数据库表历史
        var wechatAccesToken = new WechatAccessTokenInfo {
            SystemType = WechatSystemType.WechatOfficialAccount,
            AccessToken = accessToken,
            ExpiresIn = expiresIn,
        };

        session.Save(wechatAccesToken);
        session.Flush();

        TimeSpan timeout = TimeSpan.FromSeconds(result.expires_in);
        redisClient.Set(GetAccessTokenRedisKey(), accessToken, timeout);
        // 减去 5 分钟是保证redis中存储的 token 确定有效。
        redisClient.Set(GetTokenExpireRedisKey(), DateTime.Now.AddSeconds(result.expires_in - 5 * 60), timeout);

        return accessToken;
    }
}
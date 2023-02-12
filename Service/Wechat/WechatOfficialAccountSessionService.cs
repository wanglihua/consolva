using CSRedis;
using shunda.Core.Config;
using shunda.Core.Redis;
using shunda.Core.Service;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Service.Wechat;

/// <summary>
/// 微信公众号session有关的逻辑
/// todo 这里只是做个空壳，后面补上
/// </summary>
[Service(ServiceType.Scoped)]
public class WechatOfficialAccountSessionService {
    private readonly ISession session;
    private readonly WebSessionService webSessionService;

    public WechatOfficialAccountSessionService(ISession session, WebSessionService webSessionService) {
        this.session = session;
        this.webSessionService = webSessionService;
    }

    public void SetState(string state) {
        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();
        redisClient.Set(GetStateKey(), state, WebSessionConfig.LOGIN_CUSTOMER_SESSION_TIMEOUT);
    }

    public string GetState() {
        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();
        return redisClient.Get(GetStateKey());
    }

    public void SetOpenId(string openId) {
    }

    public string GetOpenId() {
        return "open-id-test-0001";
    }

    private string GetStateKey() {
        string redisKeyPrefix = RedisClientUtil.RedisKeyPrefix;
        string webSessionId = webSessionService.SessionId;

        // woa: wechat_official_account
        return $"{redisKeyPrefix}:woa_oauth_state_key:{webSessionId}"; // 一定要带有 webSessionId
    }

    private string GetOpenIdKey() {
        string redisKeyPrefix = RedisClientUtil.RedisKeyPrefix;
        string webSessionId = webSessionService.SessionId;

        // woa: wechat_official_account
        return $"{redisKeyPrefix}:woa_oauth_open_id_key:{webSessionId}"; // 一定要带有 webSessionId            
    }


    public void BindOpenIdToCustomerAndValidatePromotionNewThread(int customerId) {
        // 这个跟原来的应该有不同
    }
}
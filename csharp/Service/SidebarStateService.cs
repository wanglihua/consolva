using CSRedis;
using shunda.Core.Redis;
using shunda.Core.Service;

namespace shunda.Service;

[Service(ServiceType.Scoped)]
public class SidebarStateService
{
    private LoginUserService loginUserService;

    public SidebarStateService(LoginUserService loginUserService)
    {
        this.loginUserService = loginUserService;
    }

    public void SetState(bool isMin)
    {
        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();
        redisClient.Set(GetStateRedisKey(), isMin.ToString(), TimeSpan.FromDays(300));
    }

    public bool GetMinState()
    {
        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();
        string isMinStr = redisClient.Get(GetStateRedisKey());

        if (bool.TryParse(isMinStr, out bool isMin))
        {
            return isMin;
        }

        return true; // 默认为折叠的
    }

    private string GetStateRedisKey()
    {
        string keyPrefix = RedisClientUtil.RedisKeyPrefix;
        return $"{keyPrefix}:sidebar_state:{loginUserService.GetLoginUserId()}";
    }
}
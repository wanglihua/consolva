using CSRedis;
using shunda.Core.Redis;
using shunda.Core.Service;

namespace shunda.Service;

[Service(ServiceType.Scoped)]
public class SidebarStateService {
    public SidebarStateService() {
    }

    public void SetState(int userId, bool isMin) {
        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();
        redisClient.Set(GetStateRedisKey(userId), isMin.ToString(), TimeSpan.FromDays(300));
    }

    public bool GetMinState(int userId) {
        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();
        string isMinStr = redisClient.Get(GetStateRedisKey(userId));

        if (bool.TryParse(isMinStr, out bool isMin)) {
            return isMin;
        }

        return true; // 默认为折叠的
    }

    private string GetStateRedisKey(int userId) {
        string keyPrefix = RedisClientUtil.RedisKeyPrefix;
        return $"{keyPrefix}:sidebar_state:{userId}";
    }
}
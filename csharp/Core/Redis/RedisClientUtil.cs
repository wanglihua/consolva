using CSRedis;
using shunda.Core.Config;

namespace shunda.Core.Redis;

public class RedisClientUtil
{
    private static readonly object locker = new object();

    private static CSRedisClient? redisClient = null;

    private static void Init()
    {
        lock (locker)
        {
            if (redisClient == null)
            {
                redisClient = new CSRedisClient(RedisConfig.RedisClientConnectionString);
                RedisHelper.Initialization(redisClient);
            }
        }
    }

    public static CSRedisClient GetRedisClient()
    {
        if (redisClient == null)
        {
            Init();
        }

        return redisClient!;
    }

    /// <summary>
    /// Redis Key 的约定的本项目系统的前缀
    /// </summary>
    public static string RedisKeyPrefix
    {
        get { return "iepoch"; }
    }
}
namespace shunda.Core.Config;

public static class RedisConfig
{
    /// <summary>
    /// StackExchange.Redis
    /// </summary>
    public static string RedisDatabaseConnectionString
    {
        get
        {
#if DEBUG
            return "127.0.0.1:6379,password=,connectTimeout=5000"; // 5秒
#else
                return "127.0.0.1:6379,password=,connectTimeout=5000"; // 5秒
#endif
        }
    }

    /// <summary>
    /// CSRedis
    /// </summary>
    public static string RedisClientConnectionString
    {
        get
        {
#if DEBUG
            return "127.0.0.1:6379,pass=defaultDatabase=0,poolsize=50,ssl=false,writeBuffer=10240";
#else
                return "127.0.0.1:6379,pass=defaultDatabase=0,poolsize=50,ssl=false,writeBuffer=10240";
#endif
        }
    }
}
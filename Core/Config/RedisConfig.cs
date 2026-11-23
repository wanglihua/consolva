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
            return "127.0.0.1:6379,password=,connectTimeout=5000"; // 5秒
        }
    }

    /// <summary>
    /// CSRedis
    /// </summary>
    public static string RedisClientConnectionString
    {
        get
        {
            return "127.0.0.1:6379,pass=defaultDatabase=0,poolsize=50,ssl=false,writeBuffer=10240";
        }
    }
}
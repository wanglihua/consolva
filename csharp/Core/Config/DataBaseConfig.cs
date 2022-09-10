namespace shunda.Core.Config;

public static class DataBaseConfig
{
    public enum DataBaseType
    {
        MySQL = 100,
        SQLite = 200,
    }

    public static string MySQLConnectionString
    {
        get
        {
#if DEBUG
            return "server=127.0.0.1;database=iepoch;user id=root;password=root;MinPoolSize=4;MaxPoolSize=512;Allow User Variables=True;CharacterSet=utf8mb4;";
#else
                // return "server=rm-j6c2d7u63zust6382.mysql.rds.aliyuncs.com;database=iepoch;user id=iepoch;password=iepoch_E20190629Pp;MinPoolSize=4;MaxPoolSize=512;Allow User Variables=True;CharacterSet=utf8mb4;"; // 虚拟专有网络
                
                return "server=rm-j6c2d7u63zust6382no.mysql.rds.aliyuncs.com;database=iepoch;user id=iepoch;password=iepoch_E20190629Pp;MinPoolSize=4;MaxPoolSize=512;Allow User Variables=True;CharacterSet=utf8mb4;"; // 公网
#endif
        }
    }

    public static string SQLiteConnectionString
    {
        get
        {
            // 这个非常适合用于单元测试，只要测试中保持数据库连接
            return "data source=:memory:";
        }
    }
}
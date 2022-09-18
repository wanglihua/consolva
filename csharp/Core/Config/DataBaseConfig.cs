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
            return "server=127.0.0.1;database=shunda;user id=root;password=root;MinPoolSize=4;MaxPoolSize=512;Allow User Variables=True;CharacterSet=utf8mb4;";
#else
            return "server=127.0.0.1;database=shunda;user id=root;password=root;MinPoolSize=4;MaxPoolSize=512;Allow User Variables=True;CharacterSet=utf8mb4;";
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
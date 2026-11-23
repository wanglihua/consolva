namespace shunda.Core.Config;

public static class DataBaseConfig
{
    public enum DataBaseType
    {
        MySQL = 1000,
        SQLite = 2000,
    }

    public static string MySQLConnectionString
    {
        get
        {
            return "server=localhost;database=shunda;user id=root;password=root;MinPoolSize=4;MaxPoolSize=512;AllowBatch=True;AllowUserVariables=True;CharacterSet=utf8mb4;old guids=true;";
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
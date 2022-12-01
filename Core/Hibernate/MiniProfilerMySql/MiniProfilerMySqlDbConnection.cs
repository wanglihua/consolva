using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace shunda.Core.Hibernate.MiniProfilerMySql;

public class MiniProfilerMySqlDbConnection : ProfiledDbConnection
{
    public MySqlConnection Connection { get; set; }

    public MiniProfilerMySqlDbConnection(MySqlConnection connection, MiniProfiler miniProfiler)
        : base(connection, miniProfiler)
    {
        this.Connection = connection;
    }

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
    {
        return new MiniProfilerMySqlDbTransaction(Connection.BeginTransaction(isolationLevel), this);
    }
}
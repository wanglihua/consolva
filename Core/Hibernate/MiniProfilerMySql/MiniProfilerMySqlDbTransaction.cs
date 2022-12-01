using MySql.Data.MySqlClient;
using StackExchange.Profiling.Data;

namespace shunda.Core.Hibernate.MiniProfilerMySql;

public class MiniProfilerMySqlDbTransaction : ProfiledDbTransaction
{
    public MySqlTransaction Transaction { get; set; }

    public MiniProfilerMySqlDbTransaction(MySqlTransaction transaction, ProfiledDbConnection connection)
        : base(transaction, connection)
    {
        this.Transaction = transaction;
    }
}
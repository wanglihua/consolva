using System.Data.Common;
using MySql.Data.MySqlClient;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace shunda.Core.Hibernate.MiniProfilerMySql;

public class MiniProfilerMySqlDbCommand : ProfiledDbCommand
{
    public MySqlCommand Command { get; set; }

    private DbTransaction? transaction;

    public MiniProfilerMySqlDbCommand(MySqlCommand command, MySqlConnection connection, MiniProfiler miniProfiler)
        : base(command, connection, miniProfiler)
    {
        this.Command = command;
    }

    protected override DbTransaction? DbTransaction
    {
        get { return transaction; }
        set
        {
            this.transaction = value;
            MiniProfilerMySqlDbTransaction? miniProfilerMySqlDbTransaction = value as MiniProfilerMySqlDbTransaction;
            Command.Transaction = miniProfilerMySqlDbTransaction == null ? (MySqlTransaction?)value : miniProfilerMySqlDbTransaction.Transaction;
        }
    }
}
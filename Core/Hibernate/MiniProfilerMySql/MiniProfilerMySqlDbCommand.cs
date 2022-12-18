using System.Data.Common;
using MySql.Data.MySqlClient;
using NLog;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace shunda.Core.Hibernate.MiniProfilerMySql;

public class MiniProfilerMySqlDbCommand : ProfiledDbCommand {
    private static Logger log = LogManager.GetCurrentClassLogger();

    public MySqlCommand Command { get; set; }

    private DbTransaction? transaction;

    public MiniProfilerMySqlDbCommand(MySqlCommand command, MySqlConnection connection, MiniProfiler miniProfiler)
        : base(command, connection, miniProfiler) {
        this.Command = command;
    }

    protected override DbTransaction? DbTransaction {
        get { return transaction; }
        set {
            this.transaction = value;
            MiniProfilerMySqlDbTransaction? miniProfilerMySqlDbTransaction = value as MiniProfilerMySqlDbTransaction;
            Command.Transaction = miniProfilerMySqlDbTransaction == null ? (MySqlTransaction?)value : miniProfilerMySqlDbTransaction.Transaction;
        }
    }

    public override int ExecuteNonQuery() {
        if (this.Transaction == null) {
            throw new ValidationException("操作未启用数据库事务！");
        }

        return base.ExecuteNonQuery();
    }

    public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken) {
        if (this.Transaction == null) {
            throw new ValidationException("操作未启用数据库事务！");
        }

        return base.ExecuteNonQueryAsync(cancellationToken);
    }
}
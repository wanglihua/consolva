using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using NLog;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace shunda.Core.Hibernate.MiniProfilerMySql;

public class MiniProfilerMySqlDbCommand : ProfiledDbCommand {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

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

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) {
        if (this.Transaction == null && HasWriteOperation(this.CommandText)) {
            throw new ValidationException("操作未启用数据库事务！");
        }

        return base.ExecuteDbDataReader(behavior)!;
    }

    protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken) {
        if (this.Transaction == null && HasWriteOperation(this.CommandText)) {
            throw new ValidationException("操作未启用数据库事务！");
        }

        return base.ExecuteDbDataReaderAsync(behavior, cancellationToken);
    }

    public override object ExecuteScalar() {
        if (this.Transaction == null && HasWriteOperation(this.CommandText)) {
            throw new ValidationException("操作未启用数据库事务！");
        }

        return base.ExecuteScalar();
    }

    public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken) {
        if (this.Transaction == null && HasWriteOperation(this.CommandText)) {
            throw new ValidationException("操作未启用数据库事务！");
        }

        return base.ExecuteScalarAsync(cancellationToken);
    }

    private bool HasWriteOperation(string commandText) {
        if (string.IsNullOrWhiteSpace(commandText)) {
            commandText = string.Empty;
        }

        commandText = commandText.Trim().ToLower();

        if (commandText.Contains("insert") && commandText.Contains("into") && (commandText.Contains("values") || commandText.Contains("value"))) {
            return true;
        }

        if (commandText.Contains("update") && commandText.Contains("set") && commandText.Contains("where")) {
            return true;
        }

        if (commandText.Contains("delete") && commandText.Contains("from") && commandText.Contains("where")) {
            return true;
        }

        return false;
    }
}
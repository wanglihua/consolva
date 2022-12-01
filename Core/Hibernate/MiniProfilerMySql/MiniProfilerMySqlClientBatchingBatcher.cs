using System.Data.Common;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using NHibernate;
using NHibernate.AdoNet;
using NHibernate.AdoNet.Util;
using NHibernate.Exceptions;
using NHibernate.Transaction;

namespace shunda.Core.Hibernate.MiniProfilerMySql;

public class MiniProfilerMySqlClientBatchingBatcher : AbstractBatcher
{
    private int batchSize;
    private int totalExpectedRowsAffected;
    private MySqlClientSqlCommandSet currentBatch;
    private StringBuilder currentBatchCommandsLog;

    public MiniProfilerMySqlClientBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
        : base(connectionManager, interceptor)
    {
        batchSize = Factory.Settings.AdoBatchSize;
        currentBatch = CreateConfiguredBatch();
        currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
    }

    public override int BatchSize
    {
        get { return batchSize; }
        set { batchSize = value; }
    }

    protected override int CountOfStatementsInCurrentBatch
    {
        get { return currentBatch.CountOfCommands; }
    }

    public override void AddToBatch(IExpectation expectation)
    {
        if (CountOfStatementsInCurrentBatch == 0)
        {
            CheckReaders();
        }

        totalExpectedRowsAffected += expectation.ExpectedRowCount;
        DbCommand currentCommand = CurrentCommand;
        Prepare(currentCommand);
        Driver.AdjustCommand(currentCommand);

        string lineWithParameters = string.Empty;
        SqlStatementLogger sqlStatementLogger = Factory.Settings.SqlStatementLogger;
        if (sqlStatementLogger.IsDebugEnabled || Log.IsDebugEnabled())
        {
            lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(currentCommand);
            lineWithParameters = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic).Formatter.Format(lineWithParameters);
            currentBatchCommandsLog.Append("command ").Append(currentBatch.CountOfCommands).Append(":").AppendLine(lineWithParameters);
        }

        if (Log.IsDebugEnabled())
        {
            Log.Debug("Adding to batch:{0}", lineWithParameters);
        }

        currentBatch.Append(((MiniProfilerMySqlDbCommand)currentCommand).Command);

        if (currentBatch.CountOfCommands >= batchSize)
        {
            // DoExecuteBatch(currentCommand);
            ExecuteBatchWithTiming(currentCommand);
        }
    }

    protected void ProfiledPrepare(DbCommand cmd)
    {
        try
        {
            DbConnection sessionConnection = ConnectionManager.GetConnection();

            if (cmd.Connection != null)
            {
                // make sure the commands connection is the same as the Sessions connection
                // these can be different when the session is disconnected and then reconnected
                if (cmd.Connection != sessionConnection)
                {
                    cmd.Connection = sessionConnection;
                }
            }
            else
            {
                cmd.Connection = (sessionConnection as MiniProfilerMySqlDbConnection)?.Connection;
            }

            MiniProfilerMySqlDbTransaction? transaction = (MiniProfilerMySqlDbTransaction?)typeof(AdoTransaction).InvokeMember(
                "trans",
                BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                ConnectionManager.CurrentTransaction,
                null);

            if (transaction != null)
            {
                cmd.Transaction = transaction.Transaction;
            }

            Factory.ConnectionProvider.Driver.PrepareCommand(cmd);
        }
        catch (InvalidOperationException ioe)
        {
            throw new ADOException("While preparing " + cmd.CommandText + " an error occurred", ioe);
        }
    }

    protected override void DoExecuteBatch(DbCommand cmd)
    {
        try
        {
            Log.Debug("Executing batch");
            CheckReaders();

            ProfiledPrepare(cmd);

            if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
            {
                Factory.Settings.SqlStatementLogger.LogBatchCommand(currentBatchCommandsLog.ToString());
            }

            int rowCount;
            try
            {
                rowCount = currentBatch.ExecuteNonQuery();
            }
            catch (DbException ex)
            {
                throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, (Exception)ex, "could not execute batch command.");
            }

            Expectations.VerifyOutcomeBatched(totalExpectedRowsAffected, rowCount, cmd);
        }
        finally
        {
            ClearCurrentBatch();
        }
    }

    private MySqlClientSqlCommandSet CreateConfiguredBatch()
    {
        return new MySqlClientSqlCommandSet(batchSize);
    }

    private void ClearCurrentBatch()
    {
        currentBatch.Dispose();
        totalExpectedRowsAffected = 0;
        currentBatch = CreateConfiguredBatch();

        if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
        {
            currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
        }
    }

    public override async Task AddToBatchAsync(IExpectation expectation, CancellationToken cancellationToken)
    {
        MiniProfilerMySqlClientBatchingBatcher clientBatchingBatcher = this;
        cancellationToken.ThrowIfCancellationRequested();
        ConfiguredTaskAwaitable configuredTaskAwaitable;
        if (clientBatchingBatcher.CountOfStatementsInCurrentBatch == 0)
        {
            configuredTaskAwaitable = clientBatchingBatcher.CheckReadersAsync(cancellationToken).ConfigureAwait(false);
            await configuredTaskAwaitable;
        }

        clientBatchingBatcher.totalExpectedRowsAffected += expectation.ExpectedRowCount;
        DbCommand batchUpdate = clientBatchingBatcher.CurrentCommand;
        configuredTaskAwaitable = clientBatchingBatcher.PrepareAsync(batchUpdate, cancellationToken).ConfigureAwait(false);
        await configuredTaskAwaitable;
        clientBatchingBatcher.Driver.AdjustCommand(batchUpdate);

        string lineWithParameters = string.Empty;
        SqlStatementLogger sqlStatementLogger = clientBatchingBatcher.Factory.Settings.SqlStatementLogger;
        if (sqlStatementLogger.IsDebugEnabled || Log.IsDebugEnabled())
        {
            lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(batchUpdate);
            lineWithParameters = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic).Formatter.Format(lineWithParameters);
            clientBatchingBatcher.currentBatchCommandsLog.Append("command ").Append(clientBatchingBatcher.currentBatch.CountOfCommands).Append(":").AppendLine(lineWithParameters);
        }

        if (Log.IsDebugEnabled())
        {
            Log.Debug("Adding to batch:{0}", lineWithParameters);
        }

        clientBatchingBatcher.currentBatch.Append(((MiniProfilerMySqlDbCommand)batchUpdate).Command);
        if (clientBatchingBatcher.currentBatch.CountOfCommands >= clientBatchingBatcher.batchSize)
        {
            configuredTaskAwaitable = clientBatchingBatcher.DoExecuteBatchAsync(batchUpdate, cancellationToken).ConfigureAwait(false);
            await configuredTaskAwaitable;
        }
    }

    protected override async Task DoExecuteBatchAsync(DbCommand cmd, CancellationToken cancellationToken)
    {
        MiniProfilerMySqlClientBatchingBatcher clientBatchingBatcher = this;
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            Log.Debug("Executing batch");
            await clientBatchingBatcher.CheckReadersAsync(cancellationToken).ConfigureAwait(false);

            ProfiledPrepare(cmd);

            if (clientBatchingBatcher.Factory.Settings.SqlStatementLogger.IsDebugEnabled)
            {
                clientBatchingBatcher.Factory.Settings.SqlStatementLogger.LogBatchCommand(clientBatchingBatcher.currentBatchCommandsLog.ToString());
            }

            int rowCount;
            try
            {
                rowCount = clientBatchingBatcher.currentBatch.ExecuteNonQuery();
            }
            catch (DbException ex)
            {
                throw ADOExceptionHelper.Convert(clientBatchingBatcher.Factory.SQLExceptionConverter, (Exception)ex, "could not execute batch command.");
            }

            Expectations.VerifyOutcomeBatched(clientBatchingBatcher.totalExpectedRowsAffected, rowCount, cmd);
        }
        finally
        {
            clientBatchingBatcher.ClearCurrentBatch();
        }
    }

    public override void CloseCommands()
    {
        base.CloseCommands();
        try
        {
            ClearCurrentBatch();
        }
        catch (Exception ex)
        {
            Log.Warn(ex, "Exception clearing batch");
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        try
        {
            currentBatch.Dispose();
        }
        catch (Exception ex)
        {
            Log.Warn(ex, "Exception closing batcher");
        }
    }
}
using System.Data;
using System.Data.Common;
using NHibernate.Driver;
using NHibernate.Engine;

namespace shunda.Core.Hibernate;

public class SQLiteProfileDriver : DriverBase
{
    public override DbConnection CreateConnection()
    {
        DbConnection connection = new SQLiteProfileConnection();
        connection.StateChange += new StateChangeEventHandler(SQLiteProfileDriver.Connection_StateChange);
        return connection;
    }

    public override DbCommand CreateCommand()
    {
        return new SQLiteProfileCommand();
    }

    private static void Connection_StateChange(object sender, StateChangeEventArgs e)
    {
        if (e.OriginalState != ConnectionState.Broken && e.OriginalState != ConnectionState.Closed && e.OriginalState != ConnectionState.Connecting || e.CurrentState != ConnectionState.Open)
        {
            return;
        }

        using (DbCommand command = ((DbConnection)sender).CreateCommand())
        {
            command.CommandText = "PRAGMA foreign_keys = ON";
            command.ExecuteNonQuery();
        }
    }

    public override IResultSetsCommand GetResultSetsCommand(
        ISessionImplementor session)
    {
        return (IResultSetsCommand)new BasicResultSetsCommand(session);
    }

    public override bool UseNamedPrefixInSql => true;

    public override bool UseNamedPrefixInParameter => true;

    public override string NamedPrefix => "@";

    public override bool SupportsMultipleOpenReaders => false;

    public override bool SupportsMultipleQueries => true;

    public override bool SupportsNullEnlistment => false;

    public override bool HasDelayedDistributedTransactionCompletion => true;
}
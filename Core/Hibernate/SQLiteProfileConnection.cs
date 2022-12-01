using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace shunda.Core.Hibernate;

#pragma warning disable CS8765

public class SQLiteProfileConnection : DbConnection
{
    public SQLiteConnection InnerConnection = new SQLiteConnection();

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
    {
        return InnerConnection.BeginTransaction(isolationLevel);
    }

    public override void ChangeDatabase(string databaseName)
    {
        InnerConnection.ChangeDatabase(databaseName);
    }

    public override void Close()
    {
        InnerConnection.Close();
    }

    public override void Open()
    {
        InnerConnection.Open();
    }

    public override string ConnectionString
    {
        get => InnerConnection.ConnectionString;
        set => InnerConnection.ConnectionString = value;
    }

    public override string Database => InnerConnection.Database;

    public override ConnectionState State => InnerConnection.State;

    public override string DataSource => InnerConnection.Database;

    public override string ServerVersion => InnerConnection.ServerVersion ?? string.Empty;

    protected override DbCommand CreateDbCommand()
    {
        return new SQLiteProfileCommand() {
            Connection = this,
        };
    }
}
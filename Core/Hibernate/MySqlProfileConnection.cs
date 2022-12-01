using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace shunda.Core.Hibernate;

#pragma warning disable CS8765

public class MySqlProfileConnection : DbConnection
{
    public MySqlConnection InnerConnection = new MySqlConnection();

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
    {
        if (isolationLevel == IsolationLevel.Unspecified)
        {
            isolationLevel = IsolationLevel.ReadCommitted;
        }

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
        return new MySqlProfileCommand() {
            Connection = this,
        };
    }
}
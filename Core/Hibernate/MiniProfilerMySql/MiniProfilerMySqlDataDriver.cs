using System.Data.Common;
using MySql.Data.MySqlClient;
using NHibernate.AdoNet;
using NHibernate.Driver;
using NHibernate.Engine;
using StackExchange.Profiling;

namespace shunda.Core.Hibernate.MiniProfilerMySql;

public class MiniProfilerMySqlDataDriver : DriverBase, IEmbeddedBatcherFactoryProvider {
    public override DbConnection CreateConnection() {
        // return new MySqlConnection();
        return new MiniProfilerMySqlDbConnection(new MySqlConnection(), MiniProfiler.Current);
    }

    public override DbCommand CreateCommand() {
        // return new MySqlCommand();
        return new MiniProfilerMySqlDbCommand(new MySqlCommand(), new MySqlConnection(), MiniProfiler.Current);
    }

    public override bool UseNamedPrefixInSql => true;

    public override bool UseNamedPrefixInParameter => true;

    public override string NamedPrefix => "?";

    public override bool SupportsMultipleOpenReaders => false;

    protected override bool SupportsPreparingCommands => false;

    public override IResultSetsCommand GetResultSetsCommand(ISessionImplementor session) {
        return new BasicResultSetsCommand(session);
    }

    public override bool SupportsMultipleQueries => true;

    public override bool RequiresTimeSpanForTime => true;

    public override DateTime MinDate => new DateTime(1, 1, 1);

    Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass {
        get { return typeof(MiniProfilerMySqlClientBatchingBatcherFactory); }
    }
}
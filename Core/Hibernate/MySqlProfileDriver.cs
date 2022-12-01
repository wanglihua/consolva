using System.Data.Common;
using NHibernate.Driver;
using NHibernate.Engine;

namespace shunda.Core.Hibernate;

public class MySqlProfileDriver : DriverBase
{
    public override DbConnection CreateConnection()
    {
        return new MySqlProfileConnection();
    }

    public override DbCommand CreateCommand()
    {
        return new MySqlProfileCommand();
    }

    public override bool UseNamedPrefixInSql => true;

    public override bool UseNamedPrefixInParameter => true;

    public override string NamedPrefix => "?";

    public override bool SupportsMultipleOpenReaders => false;

    protected override bool SupportsPreparingCommands => false;

    public override IResultSetsCommand GetResultSetsCommand(ISessionImplementor session)
    {
        return new BasicResultSetsCommand(session);
    }

    public override bool SupportsMultipleQueries => true;

    public override bool RequiresTimeSpanForTime => true;

    public override DateTime MinDate => new DateTime(1, 1, 1);
}
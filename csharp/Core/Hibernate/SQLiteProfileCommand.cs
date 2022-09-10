using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using NLog;

namespace shunda.Core.Hibernate;

#pragma warning disable CS8765

public class SQLiteProfileCommand:DbCommand
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private SQLiteCommand innerCommand = new SQLiteCommand();
    
    public override void Cancel()
    {
        innerCommand.Cancel();
    }

    public override void Prepare()
    {
        innerCommand.Prepare();
    }

    public override string CommandText
    {
        get => innerCommand.CommandText;
        set => innerCommand.CommandText = value;
    }

    public override int CommandTimeout
    {
        get => innerCommand.CommandTimeout;
        set => innerCommand.CommandTimeout = value;
    }

    public override CommandType CommandType
    {
        get => innerCommand.CommandType;
        set => innerCommand.CommandType = value;
    }

    public override UpdateRowSource UpdatedRowSource
    {
        get => innerCommand.UpdatedRowSource;
        set => innerCommand.UpdatedRowSource = value;
    }

    protected override DbConnection? DbConnection
    {
        get => innerCommand.Connection;
        set => innerCommand.Connection = ((SQLiteProfileConnection)value!).InnerConnection;
    }

    protected override DbParameterCollection DbParameterCollection => innerCommand.Parameters;

    protected override DbTransaction? DbTransaction
    {
        get => innerCommand.Transaction;
        set => innerCommand.Transaction = (SQLiteTransaction)value!;
    }

    public override bool DesignTimeVisible
    {
        get => innerCommand.DesignTimeVisible;
        set => innerCommand.DesignTimeVisible = value;
    }

    protected override DbParameter CreateDbParameter()
    {
        return innerCommand.CreateParameter();
    }

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
    {
        DateTime timeBegin = DateTime.Now;
        DbDataReader reader = innerCommand.ExecuteReader(behavior);
        ProfileSQL(timeBegin);
        return reader;
    }

    public override int ExecuteNonQuery()
    {
        DateTime timeBegin = DateTime.Now;
        int rowsAffected = innerCommand.ExecuteNonQuery();
        ProfileSQL(timeBegin);
        return rowsAffected;
    }

    public override object? ExecuteScalar()
    {
        DateTime timeBegin = DateTime.Now;
        var result = innerCommand.ExecuteScalar();
        ProfileSQL(timeBegin);
        return result;
    }

    private void ProfileSQL(DateTime timeBegin)
    {
        string sqlTrace = CommandText ?? string.Empty;

        var parameterList = DbParameterCollection;
        int paramCount = DbParameterCollection.Count;
        for (int index = 0; index < paramCount; index++)
        {
            var p = parameterList[index];
            string parameterValue = p.Value?.ToString() ?? string.Empty;
            if (p.DbType == DbType.String
                || p.DbType == DbType.StringFixedLength
                || p.DbType == DbType.AnsiString
                || p.DbType == DbType.AnsiStringFixedLength
                || p.DbType == DbType.DateTime
                || p.DbType == DbType.Date
                || p.DbType == DbType.Time
                || p.DbType == DbType.DateTime2
                || p.DbType == DbType.DateTimeOffset
                || p.DbType == DbType.Guid)
            {
                parameterValue = $"'{parameterValue}'";
            }

            sqlTrace = sqlTrace.Replace(p.ParameterName, parameterValue);
        }

        sqlTrace = sqlTrace.Replace(" on ", " ON ");
        sqlTrace = sqlTrace.Replace(" is ", " IS ");
        sqlTrace = sqlTrace.Replace(" null ", " NULL ");
        sqlTrace = sqlTrace.Replace(" not ", " NOT ");
        sqlTrace = sqlTrace.Replace(" asc ", " ASC ");
        sqlTrace = sqlTrace.Replace(" asc,", " ASC,");
        sqlTrace = sqlTrace.Replace(" desc ", " DESC ");
        sqlTrace = sqlTrace.Replace(" desc,", " DESC,");
        sqlTrace = sqlTrace.Replace(" LIMIT ", $"{Environment.NewLine}LIMIT ");
        sqlTrace = sqlTrace.Replace(" limit ", $"{Environment.NewLine}LIMIT ");
        sqlTrace = sqlTrace.Replace(" FROM ", $"{Environment.NewLine}FROM ");
        sqlTrace = sqlTrace.Replace(" from ", $"{Environment.NewLine}FROM ");
        sqlTrace = sqlTrace.Replace(" WHERE ", $"{Environment.NewLine}WHERE ");
        sqlTrace = sqlTrace.Replace(" where ", $"{Environment.NewLine}WHERE ");
        sqlTrace = sqlTrace.Replace(" INNER JOIN ", $"{Environment.NewLine}    INNER JOIN ");
        sqlTrace = sqlTrace.Replace(" LEFT JOIN ", $"{Environment.NewLine}    LEFT JOIN ");
        sqlTrace = sqlTrace.Replace(" LEFT OUTER JOIN ", $"{Environment.NewLine}    LEFT OUTER JOIN ");
        sqlTrace = sqlTrace.Replace(" RIGHT JOIN ", $"{Environment.NewLine}    RIGHT JOIN ");
        sqlTrace = sqlTrace.Replace(" RIGHT OUTER JOIN ", $"{Environment.NewLine}    RIGHT OUTER JOIN ");
        sqlTrace = sqlTrace.Replace(" inner join ", $"{Environment.NewLine}    INNER JOIN ");
        sqlTrace = sqlTrace.Replace(" left join ", $"{Environment.NewLine}    LEFT JOIN ");
        sqlTrace = sqlTrace.Replace(" left outer join ", $"{Environment.NewLine}    LEFT OUTER JOIN ");
        sqlTrace = sqlTrace.Replace(" right join ", $"{Environment.NewLine}    RIGHT JOIN ");
        sqlTrace = sqlTrace.Replace(" right outer join ", $"{Environment.NewLine}    RIGHT OUTER JOIN ");
        sqlTrace = sqlTrace.Replace(" ORDER BY ", $"{Environment.NewLine}ORDER BY ");
        sqlTrace = sqlTrace.Replace(" order by ", $"{Environment.NewLine}ORDER BY ");

        sqlTrace = $"{sqlTrace};{Environment.NewLine}# SQL Execute Seconds: {(DateTime.Now - timeBegin).TotalSeconds:0.000000}";
        log.Trace(sqlTrace);
    }
}
using System.Text.RegularExpressions;
using StackExchange.Profiling;
using StackExchange.Profiling.Internal;
using StackExchange.Profiling.SqlFormatters;

namespace shunda.Core.Hibernate.MiniProfilerMySql;

public class MiniProfilerMySqlFormatter : ISqlFormatter
{
    private static readonly Regex ParamPrefixes = new Regex("[@:?].+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static bool includeTypeInfo;

    public MiniProfilerMySqlFormatter(bool includeTypeInfo = false) => MiniProfilerMySqlFormatter.includeTypeInfo = includeTypeInfo;

    public string FormatSql(string commandText, List<SqlTimingParameter> parameters)
    {
        if (parameters != null && parameters.Count != 0)
        {
            string input = commandText;
            foreach (SqlTimingParameter parameter in parameters)
            {
                string str = ParamPrefixes.IsMatch(parameter.Name) ? parameter.Name : Regex.Match(input, "([@:?])" + Regex.Escape(parameter.Name), RegexOptions.IgnoreCase).Value;
                if (str.HasValue())
                {
                    string value = this.GetParameterValue(parameter);
                    commandText = Regex.Replace(commandText, "(" + Regex.Escape(str) + ")([^0-9A-z]|$)", (MatchEvaluator)(m => value + m.Groups[2]?.ToString()), RegexOptions.IgnoreCase);
                }
            }
        }

        // 这里的替换可能会造成，SQL字符串值的不必要替换。比如一个字符串值中，正好有  left join ，但不是这个SQL语句语法的一部分
        commandText = commandText.Replace(" as ", " AS ");
        commandText = commandText.Replace(" on ", " ON ");
        commandText = commandText.Replace(" is ", " IS ");
        commandText = commandText.Replace(" null ", " NULL ");
        commandText = commandText.Replace(" not ", " NOT ");
        commandText = commandText.Replace(" asc ", " ASC ");
        commandText = commandText.Replace(" asc,", " ASC,");
        commandText = commandText.Replace(" desc ", " DESC ");
        commandText = commandText.Replace(" desc,", " DESC,");
        commandText = commandText.Replace(" LIMIT ", $"{Environment.NewLine}LIMIT ");
        commandText = commandText.Replace(" limit ", $"{Environment.NewLine}LIMIT ");
        commandText = commandText.Replace(" FROM ", $"{Environment.NewLine}FROM ");
        commandText = commandText.Replace(" from ", $"{Environment.NewLine}FROM ");
        commandText = commandText.Replace(" WHERE ", $"{Environment.NewLine}WHERE ");
        commandText = commandText.Replace(" where ", $"{Environment.NewLine}WHERE ");
        commandText = commandText.Replace(" INNER JOIN ", $"{Environment.NewLine}    INNER JOIN ");
        commandText = commandText.Replace(" LEFT JOIN ", $"{Environment.NewLine}    LEFT JOIN ");
        commandText = commandText.Replace(" LEFT OUTER JOIN ", $"{Environment.NewLine}    LEFT OUTER JOIN ");
        commandText = commandText.Replace(" RIGHT JOIN ", $"{Environment.NewLine}    RIGHT JOIN ");
        commandText = commandText.Replace(" RIGHT OUTER JOIN ", $"{Environment.NewLine}    RIGHT OUTER JOIN ");
        commandText = commandText.Replace(" inner join ", $"{Environment.NewLine}    INNER JOIN ");
        commandText = commandText.Replace(" left join ", $"{Environment.NewLine}    LEFT JOIN ");
        commandText = commandText.Replace(" left outer join ", $"{Environment.NewLine}    LEFT OUTER JOIN ");
        commandText = commandText.Replace(" right join ", $"{Environment.NewLine}    RIGHT JOIN ");
        commandText = commandText.Replace(" right outer join ", $"{Environment.NewLine}    RIGHT OUTER JOIN ");
        commandText = commandText.Replace(" ORDER BY ", $"{Environment.NewLine}ORDER BY ");
        commandText = commandText.Replace(" order by ", $"{Environment.NewLine}ORDER BY ");

        return commandText;
    }

    public string GetParameterValue(SqlTimingParameter param)
    {
        string? parameterValue = param.Value;
        string lower = (param.DbType ?? string.Empty).ToLower();
        if (lower != "string" && lower != "datetime")
        {
            if (lower == "boolean")
            {
                parameterValue = parameterValue == "True" ? "1" : (parameterValue == "False" ? "0" : null);
            }
        }
        else
        {
            parameterValue = string.Format("'{0}'", parameterValue);
        }

        if (parameterValue == null)
        {
            parameterValue = "null";
        }

        if (includeTypeInfo)
        {
            parameterValue = parameterValue + " /* " + param.Name + " DbType." + param.DbType + " */";
        }

        return parameterValue;
    }
}
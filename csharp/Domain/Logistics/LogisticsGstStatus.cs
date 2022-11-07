using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// GST状态类型
/// </summary>
public class LogisticsGstStatus : CodeTypeBase
{
    /// <summary>
    /// 启用
    /// </summary>
    public static readonly LogisticsGstStatus Enabled = new LogisticsGstStatus(100, "启用", "启用");

    /// <summary>
    /// 禁用
    /// </summary>
    public static readonly LogisticsGstStatus Disabled = new LogisticsGstStatus(200, "禁用", "禁用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsGstStatus None = new LogisticsGstStatus(0, "", "");

    private static readonly IReadOnlyList<LogisticsGstStatus> codeTypeList = new List<LogisticsGstStatus>() {
        Enabled,
        Disabled,
    };

    private static readonly Dictionary<int, LogisticsGstStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsGstStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsGstStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsGstStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    private LogisticsGstStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsGstStatusHibernateType : CodeHibernateTypeBase<LogisticsGstStatus>
{
    protected override LogisticsGstStatus GetCodeTypeById(int id)
    {
        return LogisticsGstStatus.GetById(id);
    }
}

public class LogisticsGstStatusModelBinder : CodeTypeModelBinderBase<LogisticsGstStatus>
{
    protected override LogisticsGstStatus GetCodeTypeById(int id)
    {
        return LogisticsGstStatus.GetById(id);
    }
}
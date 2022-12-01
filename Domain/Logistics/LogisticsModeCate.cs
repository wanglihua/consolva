using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流方式类别
/// </summary>
public class LogisticsModeCate : CodeTypeBase
{
    /// <summary>
    /// 客户计费, 1000
    /// </summary>
    public static readonly LogisticsModeCate Customer = new LogisticsModeCate(1000, "客户计费", "客户计费");

    /// <summary>
    /// 内部计费, 2000
    /// </summary>
    public static readonly LogisticsModeCate Inner = new LogisticsModeCate(2000, "内部计费", "内部计费");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsModeCate None = new LogisticsModeCate(0, "", "");

    private static readonly IReadOnlyList<LogisticsModeCate> codeTypeList = new List<LogisticsModeCate>() {
        Customer,
        Inner,
    };

    private static readonly Dictionary<int, LogisticsModeCate> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsModeCate GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsModeCate? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsModeCate> GetList()
    {
        return codeTypeList.ToList();
    }

    private LogisticsModeCate(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsModeCateHibernateType : CodeHibernateTypeBase<LogisticsModeCate>
{
    protected override LogisticsModeCate GetCodeTypeById(int id)
    {
        return LogisticsModeCate.GetById(id);
    }
}

public class LogisticsModeCateModelBinder : CodeTypeModelBinderBase<LogisticsModeCate>
{
    protected override LogisticsModeCate GetCodeTypeById(int id)
    {
        return LogisticsModeCate.GetById(id);
    }
}
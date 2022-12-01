using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 打包标签打印类型
/// </summary>
public class LogisticsPackLabelType : CodeTypeBase
{
    /// <summary>
    /// 不打印标签, 1000
    /// </summary>
    public static readonly LogisticsPackLabelType NoLabel = new LogisticsPackLabelType(1000, "10cm × 15cm 标签", "10cm × 15cm 标签");

    /// <summary>
    /// 10cm × 15cm 标签, 2000
    /// </summary>
    public static readonly LogisticsPackLabelType SizeW10H15 = new LogisticsPackLabelType(2000, "10cm × 15cm 标签", "10cm × 15cm 标签");

    /// <summary>
    /// 10cm × 5cm 标签, 3000
    /// </summary>
    public static readonly LogisticsPackLabelType SizeW10H5 = new LogisticsPackLabelType(3000, "10cm × 5cm 标签", "10cm × 5cm 标签");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsPackLabelType None = new LogisticsPackLabelType(0, "", "");

    private static readonly IReadOnlyList<LogisticsPackLabelType> codeTypeList = new List<LogisticsPackLabelType>() {
        NoLabel,
        SizeW10H15,
        SizeW10H5,
    };

    private static readonly Dictionary<int, LogisticsPackLabelType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsPackLabelType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsPackLabelType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsPackLabelType> GetList()
    {
        return codeTypeList.ToList();
    }

    private LogisticsPackLabelType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsPackLabelTypeHibernateType : CodeHibernateTypeBase<LogisticsPackLabelType>
{
    protected override LogisticsPackLabelType GetCodeTypeById(int id)
    {
        return LogisticsPackLabelType.GetById(id);
    }
}

public class LogisticsPackLabelTypeModelBinder : CodeTypeModelBinderBase<LogisticsPackLabelType>
{
    protected override LogisticsPackLabelType GetCodeTypeById(int id)
    {
        return LogisticsPackLabelType.GetById(id);
    }
}
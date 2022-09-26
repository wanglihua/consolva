using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流方式包裹大小件类型
/// </summary>
public class LogisticsModePackSizeType : CodeTypeBase
{
    /// <summary>
    /// 小件, 1000
    /// </summary>
    public static readonly LogisticsModePackSizeType Small = new LogisticsModePackSizeType(1000, "小件", "小件");

    /// <summary>
    /// 大件, 2000
    /// </summary>
    public static readonly LogisticsModePackSizeType Big = new LogisticsModePackSizeType(2000, "大件", "大件");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsModePackSizeType None = new LogisticsModePackSizeType(0, "", "");

    private static readonly List<LogisticsModePackSizeType> codeTypeList = new List<LogisticsModePackSizeType>() {
        Small,
        Big,
    };

    private static readonly Dictionary<int, LogisticsModePackSizeType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsModePackSizeType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsModePackSizeType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsModePackSizeType> GetList()
    {
        return codeTypeList;
    }

    private LogisticsModePackSizeType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsModePackSizeTypeHibernateType : CodeHibernateTypeBase<LogisticsModePackSizeType>
{
    protected override LogisticsModePackSizeType GetCodeTypeById(int id)
    {
        return LogisticsModePackSizeType.GetById(id);
    }
}

public class LogisticsModePackSizeTypeModelBinder : CodeTypeModelBinderBase<LogisticsModePackSizeType>
{
    protected override LogisticsModePackSizeType GetCodeTypeById(int id)
    {
        return LogisticsModePackSizeType.GetById(id);
    }
}
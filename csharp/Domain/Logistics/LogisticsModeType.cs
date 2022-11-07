using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流类型
/// </summary>
public class LogisticsModeType : CodeTypeBase
{
    /// <summary>
    /// 中澳快递, 物流到家, 1000
    /// </summary>
    public static readonly LogisticsModeType CnAuExpress = new LogisticsModeType(1000, "中澳快递", "该物流将从中国仓库直达您的澳洲收货地址。");

    /// <summary>
    /// 中美快递, 2000
    /// </summary>
    public static readonly LogisticsModeType CnUsExpress = new LogisticsModeType(2000, "中美快递", "中美快递");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsModeType None = new LogisticsModeType(0, "", "");

    private static readonly IReadOnlyList<LogisticsModeType> typeList = new List<LogisticsModeType>() {
        CnAuExpress,
        CnUsExpress,
    };

    private static readonly IDictionary<int, LogisticsModeType> typeMap = typeList.ToDictionary(e => e.Id);

    public static LogisticsModeType GetById(int id)
    {
        if (typeMap.ContainsKey(id))
        {
            return typeMap[id];
        }

        return None;
    }

    public static IList<LogisticsModeType> GetList()
    {
        return typeList.ToList();
    }

    private LogisticsModeType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsModeTypeHibernateType : CodeHibernateTypeBase<LogisticsModeType>
{
    protected override LogisticsModeType GetCodeTypeById(int id)
    {
        return LogisticsModeType.GetById(id);
    }
}

public class LogisticsModeTypeModelBinder : CodeTypeModelBinderBase<LogisticsModeType>
{
    protected override LogisticsModeType GetCodeTypeById(int id)
    {
        return LogisticsModeType.GetById(id);
    }
}
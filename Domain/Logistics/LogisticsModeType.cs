using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流类型
/// </summary>
public class LogisticsModeType : CodeTypeBase {
    /// <summary>
    /// 跨国快递, 1000
    /// </summary>
    public static readonly LogisticsModeType Express = new LogisticsModeType(1000, "跨国快递", "跨国快递");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsModeType None = new LogisticsModeType(0, "", "");

    private static readonly IReadOnlyList<LogisticsModeType> typeList = new List<LogisticsModeType>() {
        Express,
    };

    private static readonly IDictionary<int, LogisticsModeType> typeMap = typeList.ToDictionary(e => e.Id);

    public static LogisticsModeType GetById(int id) {
        if (typeMap.ContainsKey(id)) {
            return typeMap[id];
        }

        return None;
    }

    public static IList<LogisticsModeType> GetList() {
        return typeList.ToList();
    }

    private LogisticsModeType(int id, string name, string description) : base(id, name, description) {
    }
}

public class LogisticsModeTypeHibernateType : CodeHibernateTypeBase<LogisticsModeType> {
    protected override LogisticsModeType GetCodeTypeById(int id) {
        return LogisticsModeType.GetById(id);
    }
}

public class LogisticsModeTypeModelBinder : CodeTypeModelBinderBase<LogisticsModeType> {
    protected override LogisticsModeType GetCodeTypeById(int id) {
        return LogisticsModeType.GetById(id);
    }
}
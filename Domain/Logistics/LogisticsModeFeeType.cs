using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流方式费用计算方式
/// </summary>
public class LogisticsModeFeeType : CodeTypeBase {
    /// <summary>
    /// 汇总计算, 1000
    /// </summary>
    public static readonly LogisticsModeFeeType Summary = new LogisticsModeFeeType(1000, "汇总计算", "汇总计算");
    
    /// <summary>
    /// 按箱号独立计算, 2000
    /// </summary>
    public static readonly LogisticsModeFeeType SinglePackBox = new LogisticsModeFeeType(2000, "按箱号独立计算", "按箱号独立计算");    

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsModeFeeType None = new LogisticsModeFeeType(0, "", "");

    private static readonly IReadOnlyList<LogisticsModeFeeType> typeList = new List<LogisticsModeFeeType>() {
        Summary,
        SinglePackBox,
    };

    private static readonly IDictionary<int, LogisticsModeFeeType> typeMap = typeList.ToDictionary(e => e.Id);

    public static LogisticsModeFeeType GetById(int id) {
        if (typeMap.ContainsKey(id)) {
            return typeMap[id];
        }

        return None;
    }

    public static IList<LogisticsModeFeeType> GetList() {
        return typeList.ToList();
    }

    private LogisticsModeFeeType(int id, string name, string description) : base(id, name, description) {
    }
}

public class LogisticsModeFeeTypeHibernateType : CodeHibernateTypeBase<LogisticsModeFeeType> {
    protected override LogisticsModeFeeType GetCodeTypeById(int id) {
        return LogisticsModeFeeType.GetById(id);
    }
}

public class LogisticsModeFeeTypeModelBinder : CodeTypeModelBinderBase<LogisticsModeFeeType> {
    protected override LogisticsModeFeeType GetCodeTypeById(int id) {
        return LogisticsModeFeeType.GetById(id);
    }
}
using shunda.Core.CodeType;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 订单备注类型
/// </summary>
public class LogisticsOrderRemarkType : CodeTypeBase {
    /// <summary>
    /// 客户备注, 1000
    /// </summary>
    public static readonly LogisticsOrderRemarkType Customer = new LogisticsOrderRemarkType(1000, "客户备注", "客户备注");

    /// <summary>
    /// 仓库备注, 2000
    /// </summary>
    public static readonly LogisticsOrderRemarkType Warehouse = new LogisticsOrderRemarkType(2000, "仓库备注", "仓库备注");

    /// <summary>
    /// 客服备注, 3000
    /// </summary>
    public static readonly LogisticsOrderRemarkType Service = new LogisticsOrderRemarkType(3000, "客服备注", "客服备注");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderRemarkType None = new LogisticsOrderRemarkType(0, "", "");

    private static readonly IReadOnlyList<LogisticsOrderRemarkType> codeTypeList = new List<LogisticsOrderRemarkType>() {
        Customer,
        Warehouse,
        Service,
    };

    private static readonly Dictionary<int, LogisticsOrderRemarkType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsOrderRemarkType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out LogisticsOrderRemarkType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static List<LogisticsOrderRemarkType> GetList() {
        return codeTypeList.ToList();
    }

    private LogisticsOrderRemarkType(int id, string name, string description) : base(id, name, description) {
    }
}

public class LogisticsOrderRemarkTypeHibernateType : CodeHibernateTypeBase<LogisticsOrderRemarkType> {
    protected override LogisticsOrderRemarkType GetCodeTypeById(int id) {
        return LogisticsOrderRemarkType.GetById(id);
    }
}

public class LogisticsOrderRemarkTypeModelBinder : CodeTypeModelBinderBase<LogisticsOrderRemarkType> {
    protected override LogisticsOrderRemarkType GetCodeTypeById(int id) {
        return LogisticsOrderRemarkType.GetById(id);
    }
}
using shunda.Core.CodeType;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单备注类型
/// </summary>
public class ExpressRemarkType : CodeTypeBase {
    /// <summary>
    /// 客户备注, 1000
    /// </summary>
    public static readonly ExpressRemarkType Customer = new ExpressRemarkType(1000, "客户备注", "客户备注");

    /// <summary>
    /// 入库备注, 2000
    /// </summary>
    public static readonly ExpressRemarkType Warehouse = new ExpressRemarkType(2000, "入库备注", "入库备注");

    /// <summary>
    /// 客服备注, 3000
    /// </summary>
    public static readonly ExpressRemarkType Service = new ExpressRemarkType(3000, "客服备注", "客服备注");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressRemarkType None = new ExpressRemarkType(0, "", "");

    private static readonly IReadOnlyList<ExpressRemarkType> codeTypeList = new List<ExpressRemarkType>() {
        Customer,
        Warehouse,
        Service,
    };

    private static readonly Dictionary<int, ExpressRemarkType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ExpressRemarkType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out ExpressRemarkType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static List<ExpressRemarkType> GetList() {
        return codeTypeList.ToList();
    }

    private ExpressRemarkType(int id, string name, string description) : base(id, name, description) {
    }
}

public class ExpressRemarkTypeHibernateType : CodeHibernateTypeBase<ExpressRemarkType> {
    protected override ExpressRemarkType GetCodeTypeById(int id) {
        return ExpressRemarkType.GetById(id);
    }
}

public class ExpressRemarkTypeModelBinder : CodeTypeModelBinderBase<ExpressRemarkType> {
    protected override ExpressRemarkType GetCodeTypeById(int id) {
        return ExpressRemarkType.GetById(id);
    }
}
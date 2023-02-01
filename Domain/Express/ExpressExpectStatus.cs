using shunda.Core.CodeType;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单预告状态
/// </summary>
public class ExpressExpectStatus : CodeTypeBase {
    /// <summary>
    /// 未到货
    /// </summary>
    public static readonly ExpressExpectStatus Create = new ExpressExpectStatus(1000, "未到货", "未到货");

    /// <summary>
    /// 可认领
    /// 录货值
    /// </summary>
    public static readonly ExpressExpectStatus Enter = new ExpressExpectStatus(2000, "录货值", "录货值");

    /// <summary>
    /// 已入库
    /// </summary>
    public static readonly ExpressExpectStatus Checkin = new ExpressExpectStatus(3000, "已入库", "已入库");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressExpectStatus None = new ExpressExpectStatus(0, "", "");

    private static readonly IReadOnlyList<ExpressExpectStatus> codeTypeList = new List<ExpressExpectStatus>() {
        Create,
        Enter,
        Checkin,
    };

    private static readonly IDictionary<int, ExpressExpectStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ExpressExpectStatus GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out ExpressExpectStatus? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<ExpressExpectStatus> GetList() {
        return codeTypeList.ToList();
    }

    private ExpressExpectStatus(int id, string name, string description) : base(id, name, description) {
    }
}

public class ExpressExpectStatusHibernateType : CodeHibernateTypeBase<ExpressExpectStatus> {
    protected override ExpressExpectStatus GetCodeTypeById(int id) {
        return ExpressExpectStatus.GetById(id);
    }
}

public class ExpressExpectStatusModelBinder : CodeTypeModelBinderBase<ExpressExpectStatus> {
    protected override ExpressExpectStatus GetCodeTypeById(int id) {
        return ExpressExpectStatus.GetById(id);
    }
}
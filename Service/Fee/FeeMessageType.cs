using shunda.Core.CodeType;

namespace shunda.Service.Fee;

/// <summary>
/// 费用消息类型
/// </summary>
public class FeeMessageType : CodeTypeBase {
    /// <summary>
    /// 会员消息类型
    /// </summary>
    public static readonly FeeMessageType Customer = new FeeMessageType(100, "会员消息类型", "客户端会员消息类型");

    /// <summary>
    /// 后台用户消息类型
    /// </summary>
    public static readonly FeeMessageType User = new FeeMessageType(200, "用户消息类型", "管理端用户消息类型");

    /// <summary>
    /// None
    /// </summary>
    public static readonly FeeMessageType None = new FeeMessageType(0, "", "");

    private static readonly IReadOnlyList<FeeMessageType> codeTypeList = new List<FeeMessageType>() {
        Customer,
        User,
    };

    private static readonly IDictionary<int, FeeMessageType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static FeeMessageType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out FeeMessageType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<FeeMessageType> GetList() {
        return codeTypeList.ToList();
    }

    private FeeMessageType(int id, string name, string description) : base(id, name, description) {
    }
}

public class FeeMessageTypeHibernateType : CodeHibernateTypeBase<FeeMessageType> {
    protected override FeeMessageType GetCodeTypeById(int id) {
        return FeeMessageType.GetById(id);
    }
}

public class FeeMessageTypeModelBinder : CodeTypeModelBinderBase<FeeMessageType> {
    protected override FeeMessageType GetCodeTypeById(int id) {
        return FeeMessageType.GetById(id);
    }
}
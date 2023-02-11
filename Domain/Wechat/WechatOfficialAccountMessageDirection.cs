using shunda.Core.CodeType;

namespace shunda.Domain.Wechat;

public class WechatOfficialAccountMessageDirection : CodeTypeBase {
    /// <summary>
    /// 会员问询
    /// </summary>
    public static readonly WechatOfficialAccountMessageDirection CustomerToUser = new WechatOfficialAccountMessageDirection(1000, "会员问询", "会员问询");

    /// <summary>
    /// 客服回复
    /// </summary>
    public static readonly WechatOfficialAccountMessageDirection UserToCustomer = new WechatOfficialAccountMessageDirection(2000, "客服回复", "客服回复");

    /// <summary>
    /// None
    /// </summary>
    public static readonly WechatOfficialAccountMessageDirection None = new WechatOfficialAccountMessageDirection(0, "", "");

    private static readonly IReadOnlyList<WechatOfficialAccountMessageDirection> codeTypeList = new List<WechatOfficialAccountMessageDirection>() {
        CustomerToUser,
        UserToCustomer,
    };

    private static readonly IDictionary<int, WechatOfficialAccountMessageDirection> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static WechatOfficialAccountMessageDirection GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out WechatOfficialAccountMessageDirection? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<WechatOfficialAccountMessageDirection> GetList() {
        return codeTypeList.ToList();
    }

    private WechatOfficialAccountMessageDirection(int id, string name, string description) : base(id, name, description) {
    }
}

public class WechatOfficialAccountMessageDirectionHibernateType : CodeHibernateTypeBase<WechatOfficialAccountMessageDirection> {
    protected override WechatOfficialAccountMessageDirection GetCodeTypeById(int id) {
        return WechatOfficialAccountMessageDirection.GetById(id);
    }
}

public class WechatOfficialAccountMessageDirectionModelBinder : CodeTypeModelBinderBase<WechatOfficialAccountMessageDirection> {
    protected override WechatOfficialAccountMessageDirection GetCodeTypeById(int id) {
        return WechatOfficialAccountMessageDirection.GetById(id);
    }
}
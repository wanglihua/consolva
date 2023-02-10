using shunda.Core.CodeType;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信系统类型
/// </summary>
public class WechatSystemType : CodeTypeBase {
    /// <summary>
    /// 微信公众号, 1000
    /// </summary>
    public static readonly WechatSystemType WechatOfficialAccount = new WechatSystemType(1000, "微信公众号", "微信公众号");

    /// <summary>
    /// 微信小程序, 2000
    /// </summary>
    public static readonly WechatSystemType WechatApp = new WechatSystemType(2000, "微信小程序", "微信小程序");

    /// <summary>
    /// None
    /// </summary>
    public static readonly WechatSystemType None = new WechatSystemType(0, "", "");

    private static readonly IReadOnlyList<WechatSystemType> codeTypeList = new List<WechatSystemType>() {
        WechatOfficialAccount,
        WechatApp,
    };

    private static readonly Dictionary<int, WechatSystemType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static WechatSystemType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out WechatSystemType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static List<WechatSystemType> GetList() {
        return codeTypeList.ToList();
    }

    private WechatSystemType(int id, string name, string description) : base(id, name, description) {
    }
}

public class WechatAccessTokenTypeHibernateType : CodeHibernateTypeBase<WechatSystemType> {
    protected override WechatSystemType GetCodeTypeById(int id) {
        return WechatSystemType.GetById(id);
    }
}

public class WechatAccessTokenTypeModelBinder : CodeTypeModelBinderBase<WechatSystemType> {
    protected override WechatSystemType GetCodeTypeById(int id) {
        return WechatSystemType.GetById(id);
    }
}
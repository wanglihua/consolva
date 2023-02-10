using shunda.Core.CodeType;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信AccessToken类型
/// </summary>
public class WechatAccessTokenType : CodeTypeBase {
    /// <summary>
    /// 微信公众号, 1000
    /// </summary>
    public static readonly WechatAccessTokenType WechatOfficialAccount = new WechatAccessTokenType(1000, "微信公众号", "微信公众号");

    /// <summary>
    /// 微信小程序, 2000
    /// </summary>
    public static readonly WechatAccessTokenType WechatApp = new WechatAccessTokenType(2000, "微信小程序", "微信小程序");

    /// <summary>
    /// None
    /// </summary>
    public static readonly WechatAccessTokenType None = new WechatAccessTokenType(0, "", "");

    private static readonly IReadOnlyList<WechatAccessTokenType> codeTypeList = new List<WechatAccessTokenType>() {
        WechatOfficialAccount,
        WechatApp,
    };

    private static readonly Dictionary<int, WechatAccessTokenType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static WechatAccessTokenType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out WechatAccessTokenType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static List<WechatAccessTokenType> GetList() {
        return codeTypeList.ToList();
    }

    private WechatAccessTokenType(int id, string name, string description) : base(id, name, description) {
    }
}

public class WechatAccessTokenTypeHibernateType : CodeHibernateTypeBase<WechatAccessTokenType> {
    protected override WechatAccessTokenType GetCodeTypeById(int id) {
        return WechatAccessTokenType.GetById(id);
    }
}

public class WechatAccessTokenTypeModelBinder : CodeTypeModelBinderBase<WechatAccessTokenType> {
    protected override WechatAccessTokenType GetCodeTypeById(int id) {
        return WechatAccessTokenType.GetById(id);
    }
}
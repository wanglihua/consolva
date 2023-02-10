using shunda.Core.CodeType;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信公众号回调类型
/// </summary>
public class WechatCallbackHttpMethod : CodeTypeBase {
    /// <summary>
    /// Get, 1000
    /// </summary>
    public static readonly WechatCallbackHttpMethod Get = new WechatCallbackHttpMethod(1000, "Get", "Get");

    /// <summary>
    /// Post, 2000
    /// </summary>
    public static readonly WechatCallbackHttpMethod Post = new WechatCallbackHttpMethod(2000, "Post", "Post");

    /// <summary>
    /// None
    /// </summary>
    public static readonly WechatCallbackHttpMethod None = new WechatCallbackHttpMethod(0, "", "");

    private static readonly IReadOnlyList<WechatCallbackHttpMethod> codeTypeList = new List<WechatCallbackHttpMethod>() {
        Get,
        Post,
    };

    private static readonly IDictionary<int, WechatCallbackHttpMethod> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static WechatCallbackHttpMethod GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out WechatCallbackHttpMethod? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<WechatCallbackHttpMethod> GetList() {
        return codeTypeList.ToList();
    }

    private WechatCallbackHttpMethod(int id, string name, string description) : base(id, name, description) {
    }
}

public class WechatCallbackHttpMethodHibernateType : CodeHibernateTypeBase<WechatCallbackHttpMethod> {
    protected override WechatCallbackHttpMethod GetCodeTypeById(int id) {
        return WechatCallbackHttpMethod.GetById(id);
    }
}

public class WechatCallbackHttpMethodModelBinder : CodeTypeModelBinderBase<WechatCallbackHttpMethod> {
    protected override WechatCallbackHttpMethod GetCodeTypeById(int id) {
        return WechatCallbackHttpMethod.GetById(id);
    }
}
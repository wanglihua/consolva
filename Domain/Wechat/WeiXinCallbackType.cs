using shunda.Core.CodeType;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信公众号回调类型
/// </summary>
public class WeiXinCallbackType : CodeTypeBase {
    /// <summary>
    /// Get, 1000
    /// </summary>
    public static readonly WeiXinCallbackType Get = new WeiXinCallbackType(1000, "Get", "Get");

    /// <summary>
    /// Post, 2000
    /// </summary>
    public static readonly WeiXinCallbackType Post = new WeiXinCallbackType(2000, "Post", "Post");

    /// <summary>
    /// None
    /// </summary>
    public static readonly WeiXinCallbackType None = new WeiXinCallbackType(0, "", "");

    private static readonly IReadOnlyList<WeiXinCallbackType> codeTypeList = new List<WeiXinCallbackType>() {
        Get,
        Post,
    };

    private static readonly IDictionary<int, WeiXinCallbackType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static WeiXinCallbackType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out WeiXinCallbackType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<WeiXinCallbackType> GetList() {
        return codeTypeList.ToList();
    }

    private WeiXinCallbackType(int id, string name, string description) : base(id, name, description) {
    }
}

public class WeiXinCallbackTypeHibernateType : CodeHibernateTypeBase<WeiXinCallbackType> {
    protected override WeiXinCallbackType GetCodeTypeById(int id) {
        return WeiXinCallbackType.GetById(id);
    }
}

public class WeiXinCallbackTypeModelBinder : CodeTypeModelBinderBase<WeiXinCallbackType> {
    protected override WeiXinCallbackType GetCodeTypeById(int id) {
        return WeiXinCallbackType.GetById(id);
    }
}
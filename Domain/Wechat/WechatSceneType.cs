using shunda.Core.CodeType;

namespace shunda.Domain.Wechat;

public class WechatSceneType : CodeTypeBase {
    /// <summary>
    /// 客户推广
    /// </summary>
    public static readonly WechatSceneType CustomerPromotion = new WechatSceneType(1000, "客户推广", "客户推广");

    /// <summary>
    /// None
    /// </summary>
    public static readonly WechatSceneType None = new WechatSceneType(0, "", "");

    private static readonly IReadOnlyList<WechatSceneType> codeTypeList = new List<WechatSceneType>() {
        CustomerPromotion,
    };

    private static readonly IDictionary<int, WechatSceneType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static WechatSceneType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out WechatSceneType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<WechatSceneType> GetList() {
        return codeTypeList.ToList();
    }

    private WechatSceneType(int id, string name, string description) : base(id, name, description) {
    }
}

public class WechatSceneTypeHibernateType : CodeHibernateTypeBase<WechatSceneType> {
    protected override WechatSceneType GetCodeTypeById(int id) {
        return WechatSceneType.GetById(id);
    }
}

public class WechatSceneTypeModelBinder : CodeTypeModelBinderBase<WechatSceneType> {
    protected override WechatSceneType GetCodeTypeById(int id) {
        return WechatSceneType.GetById(id);
    }
}
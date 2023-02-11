using shunda.Core.CodeType;

namespace shunda.Domain.Wechat;

public class WechatOfficialAccountReplyType : CodeTypeBase {
    /// <summary>
    /// 公众号关注回复
    /// </summary>
    public static readonly WechatOfficialAccountReplyType FollowReply = new WechatOfficialAccountReplyType(1000, "公众号关注回复", "公众号关注回复");

    /// <summary>
    /// 文字信息默认回复
    /// </summary>
    public static readonly WechatOfficialAccountReplyType TextMessageReply = new WechatOfficialAccountReplyType(2000, "文字信息默认回复", "文字信息默认回复");

    /// <summary>
    /// None
    /// </summary>
    public static readonly WechatOfficialAccountReplyType None = new WechatOfficialAccountReplyType(0, "", "");

    private static readonly IReadOnlyList<WechatOfficialAccountReplyType> codeTypeList = new List<WechatOfficialAccountReplyType>() {
        FollowReply,
        TextMessageReply,
    };

    private static readonly IDictionary<int, WechatOfficialAccountReplyType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static WechatOfficialAccountReplyType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out WechatOfficialAccountReplyType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<WechatOfficialAccountReplyType> GetList() {
        return codeTypeList.ToList();
    }

    private WechatOfficialAccountReplyType(int id, string name, string description) : base(id, name, description) {
    }
}

public class WechatOfficialAccountReplyTypeHibernateType : CodeHibernateTypeBase<WechatOfficialAccountReplyType> {
    protected override WechatOfficialAccountReplyType GetCodeTypeById(int id) {
        return WechatOfficialAccountReplyType.GetById(id);
    }
}

public class WechatOfficialAccountReplyTypeModelBinder : CodeTypeModelBinderBase<WechatOfficialAccountReplyType> {
    protected override WechatOfficialAccountReplyType GetCodeTypeById(int id) {
        return WechatOfficialAccountReplyType.GetById(id);
    }
}
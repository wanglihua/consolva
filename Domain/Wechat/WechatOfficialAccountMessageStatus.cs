using shunda.Core.CodeType;

namespace shunda.Domain.Wechat;

public class WechatOfficialAccountMessageStatus : CodeTypeBase {
    /// <summary>
    /// 未回复
    /// </summary>
    public static readonly WechatOfficialAccountMessageStatus NotReply = new WechatOfficialAccountMessageStatus(1000, "未回复", "未回复");

    /// <summary>
    /// 已回复
    /// </summary>
    public static readonly WechatOfficialAccountMessageStatus Replied = new WechatOfficialAccountMessageStatus(2000, "已回复", "已回复");

    /// <summary>
    /// None
    /// </summary>
    public static readonly WechatOfficialAccountMessageStatus None = new WechatOfficialAccountMessageStatus(0, "", "");

    private static readonly IReadOnlyList<WechatOfficialAccountMessageStatus> codeTypeList = new List<WechatOfficialAccountMessageStatus>() {
        NotReply,
        Replied,
    };

    private static readonly IDictionary<int, WechatOfficialAccountMessageStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static WechatOfficialAccountMessageStatus GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out WechatOfficialAccountMessageStatus? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<WechatOfficialAccountMessageStatus> GetList() {
        return codeTypeList.ToList();
    }

    private WechatOfficialAccountMessageStatus(int id, string name, string description) : base(id, name, description) {
    }
}

public class WechatOfficialAccountMessageStatusHibernateType : CodeHibernateTypeBase<WechatOfficialAccountMessageStatus> {
    protected override WechatOfficialAccountMessageStatus GetCodeTypeById(int id) {
        return WechatOfficialAccountMessageStatus.GetById(id);
    }
}

public class WechatOfficialAccountMessageStatusModelBinder : CodeTypeModelBinderBase<WechatOfficialAccountMessageStatus> {
    protected override WechatOfficialAccountMessageStatus GetCodeTypeById(int id) {
        return WechatOfficialAccountMessageStatus.GetById(id);
    }
}
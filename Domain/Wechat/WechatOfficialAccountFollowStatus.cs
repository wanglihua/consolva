using shunda.Core.CodeType;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信关注状态
/// </summary>
public class WechatOfficialAccountFollowStatus : CodeTypeBase {
    /// <summary>
    /// 未关注
    /// </summary>
    public static readonly WechatOfficialAccountFollowStatus NotFollow = new WechatOfficialAccountFollowStatus(100, "未关注", "未关注");

    /// <summary>
    /// 已关注
    /// </summary>
    public static readonly WechatOfficialAccountFollowStatus Followed = new WechatOfficialAccountFollowStatus(200, "已关注", "已关注");

    /// <summary>
    /// 取消关注
    /// </summary>
    public static readonly WechatOfficialAccountFollowStatus UnFollowed = new WechatOfficialAccountFollowStatus(300, "取消关注", "取消关注");

    /// <summary>
    /// None
    /// </summary>
    private static readonly WechatOfficialAccountFollowStatus None = new WechatOfficialAccountFollowStatus(0, "", "");

    private static readonly IReadOnlyList<WechatOfficialAccountFollowStatus> codeTypeList = new List<WechatOfficialAccountFollowStatus>() {
        NotFollow,
        Followed,
        UnFollowed,
    };

    private static readonly IDictionary<int, WechatOfficialAccountFollowStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static WechatOfficialAccountFollowStatus GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out WechatOfficialAccountFollowStatus? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<WechatOfficialAccountFollowStatus> GetList() {
        return codeTypeList.ToList();
    }

    private WechatOfficialAccountFollowStatus(int id, string name, string description) : base(id, name, description) {
    }
}

public class WechatOfficialAccountFollowStatusHibernateType : CodeHibernateTypeBase<WechatOfficialAccountFollowStatus> {
    protected override WechatOfficialAccountFollowStatus GetCodeTypeById(int id) {
        return WechatOfficialAccountFollowStatus.GetById(id);
    }
}

public class WechatOfficialAccountFollowStatusModelBinder : CodeTypeModelBinderBase<WechatOfficialAccountFollowStatus> {
    protected override WechatOfficialAccountFollowStatus GetCodeTypeById(int id) {
        return WechatOfficialAccountFollowStatus.GetById(id);
    }
}
using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信公众号关注者
/// </summary>
public class WechatOfficialAccountFollowerInfo : Entity {

    /// <summary>
    /// 微信昵称，可空
    /// </summary>
    public virtual string? NickName { get; set; } = null;

    /// <summary>
    /// open id，非空
    /// </summary>
    public virtual string OpenId { get; set; } = string.Empty;

    /// <summary>
    /// union id，非空
    /// </summary>
    public virtual string UnionId { get; set; } = string.Empty;

    /// <summary>
    /// 关注状态，非空
    /// </summary>
    public virtual WechatOfficialAccountFollowStatus FollowStatus { get; set; } = WechatOfficialAccountFollowStatus.None;

    /// <summary>
    /// 关注时间，可空
    /// </summary>
    public virtual DateTime? FollowTime { get; set; } = null;

    /// <summary>
    /// 取消关注时间，可空
    /// </summary>
    public virtual DateTime? UnfollowTime { get; set; } = null;

    public override void Validate() {
        ValidationUtil.ValidateEntity<WechatOfficialAccountFollowerInfoValidator, WechatOfficialAccountFollowerInfo>(this);
    }
}
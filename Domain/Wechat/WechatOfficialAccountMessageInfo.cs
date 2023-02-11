using shunda.Core;
using shunda.Core.Util;
using shunda.Domain.Customer;
using shunda.Domain.Pass;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信公众号消息
/// </summary>
public class WechatOfficialAccountMessageInfo : Entity {
    /// <summary>
    /// 消息方向，非空
    /// </summary>
    public virtual WechatOfficialAccountMessageDirection Direction { get; set; } = WechatOfficialAccountMessageDirection.None;

    /// <summary>
    /// OpenId，非空
    /// </summary>
    public virtual string OpenId { get; set; } = string.Empty;

    /// <summary>
    /// 会员Id，可空
    /// </summary>
    public virtual CustomerInfo? Customer { get; set; } = null;

    /// <summary>
    /// 用户Id，可空
    /// </summary>
    public virtual UserInfo? User { get; set; } = null;

    /// <summary>
    /// 消息，非空
    /// </summary>
    public virtual string Message { get; set; } = string.Empty;

    /// <summary>
    /// 状态，非空
    /// </summary>
    public virtual WechatOfficialAccountMessageStatus Status { get; set; } = WechatOfficialAccountMessageStatus.None;

    public override void Validate() {
        ValidationUtil.ValidateEntity<WechatOfficialAccountMessageInfoValidator, WechatOfficialAccountMessageInfo>(this);
    }
}
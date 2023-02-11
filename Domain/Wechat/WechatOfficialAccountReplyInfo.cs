using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信公众号自动回复设置
/// </summary>
public class WechatOfficialAccountReplyInfo : Entity {
    /// <summary>
    /// 回复类型，非空
    /// </summary>
    public virtual WechatOfficialAccountReplyType Type { get; set; } = WechatOfficialAccountReplyType.None;

    /// <summary>
    /// 回复内容，非空
    /// </summary>
    public virtual string Content { get; set; } = string.Empty;

    public override void Validate() {
        ValidationUtil.ValidateEntity<WechatOfficialAccountReplyInfoValidator,  WechatOfficialAccountReplyInfo>(this);
    }
}
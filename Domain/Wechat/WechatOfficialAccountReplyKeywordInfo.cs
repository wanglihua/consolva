using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信公众号关键字回复设置
/// </summary>
public class WechatOfficialAccountReplyKeywordInfo : Entity {
    /// <summary>
    /// 关键字，非空
    /// </summary>
    public virtual string Keyword { get; set; } = string.Empty;

    /// <summary>
    /// 回复内容，非空
    /// </summary>
    public virtual string Content { get; set; } = string.Empty;

    public override void Validate() {
        ValidationUtil.ValidateEntity<WechatOfficialAccountReplyKeywordInfoValidator,  WechatOfficialAccountReplyKeywordInfo>(this);
    }
}
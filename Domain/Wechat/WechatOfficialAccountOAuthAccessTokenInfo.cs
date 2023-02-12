using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信公众号OAuth专用AccessToken
/// </summary>
public class WechatOfficialAccountOAuthAccessTokenInfo : EntityNoUpdate {
    /// <summary>
    /// code，非空
    /// </summary>
    public virtual string Code { get; set; } = string.Empty;

    /// <summary>
    /// access token，非空
    /// </summary>
    public virtual string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// open id，非空
    /// </summary>
    public virtual string OpenId { get; set; } = string.Empty;

    public override void Validate() {
        ValidationUtil.ValidateEntity<WechatOfficialAccountOAuthAccessTokenInfoValidator, WechatOfficialAccountOAuthAccessTokenInfo>(this);
    }
}
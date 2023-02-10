using Newtonsoft.Json;
using shunda.Core;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信AccessToken
/// </summary>
public class WechatAccessTokenInfo : EntityNoUpdate {
    /// <summary>
    /// 微信系统类型，非空
    /// </summary>
    [JsonProperty("systemType")]
    public virtual WechatSystemType SystemType { get; set; } = WechatSystemType.None;

    /// <summary>
    /// access token，非空
    /// </summary>
    [JsonProperty("accessToken")]
    public virtual string AccessToken { get; set; } = string.Empty;
    
    /// <summary>
    /// 过期时间(秒)
    /// </summary>
    public virtual int ExpiresIn { get; set; }
}
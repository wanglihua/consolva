using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using shunda.Core;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信AccessToken
/// </summary>
public class WechatAccessTokenInfo : EntityNoUpdate
{
    /// <summary>
    /// access token，非空
    /// </summary>
    [Required(ErrorMessage = "access token不能为空！")]
    [MaxLength(600, ErrorMessage = "access token长度不能大于600！")]
    [JsonProperty("accessToken")]
    public virtual string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 过期时间，非空
    /// </summary>
    [Required(ErrorMessage = "过期时间不能为空！")]
    [JsonProperty("expire")]
    public virtual DateTime Expire { get; set; } = DateTime.MinValue;

}
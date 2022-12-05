using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户登录日志
/// </summary>
public class CustomerLoginLogInfo : Entity
{
    /// <summary>
    /// 登录阶段，非空
    /// </summary>
    [Required(ErrorMessage = "登录阶段不能为空！")]
    [JsonProperty("stageId")]
    [ModelBinder(typeof(CustomerLoginStageModelBinder))]
    public virtual CustomerLoginStage Stage { get; set; } = CustomerLoginStage.None;

    /// <summary>
    /// 登录客户，非空
    /// </summary>
    [Required(ErrorMessage = "登录客户不能为空！")]
    [JsonProperty("customer")]
    public virtual CustomerInfo Customer { get; set; } = new ();

    /// <summary>
    /// open id，非空
    /// </summary>
    [Required(ErrorMessage = "open id不能为空！")]
    [MaxLength(100, ErrorMessage = "open id长度不能大于100！")]
    [JsonProperty("openId")]
    public virtual string OpenId { get; set; } = string.Empty;

    /// <summary>
    /// session id，非空
    /// </summary>
    [Required(ErrorMessage = "session id不能为空！")]
    [MaxLength(50, ErrorMessage = "session id长度不能大于50！")]
    [JsonProperty("sessionId")]
    public virtual string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// IP地址，非空
    /// </summary>
    [Required(ErrorMessage = "IP地址不能为空！")]
    [MaxLength(200, ErrorMessage = "IP地址长度不能大于200！")]
    [JsonProperty("ipAddress")]
    public virtual string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// User Agent，非空
    /// </summary>
    [Required(ErrorMessage = "User Agent不能为空！")]
    [MaxLength(300, ErrorMessage = "User Agent长度不能大于300！")]
    [JsonProperty("userAgent")]
    public virtual string UserAgent { get; set; } = string.Empty;

    /// <summary>
    /// 登录时间，非空
    /// </summary>
    [Required(ErrorMessage = "登录时间不能为空！")]
    [JsonProperty("loginTime")]
    public virtual DateTime LoginTime { get; set; } = DateTime.MinValue;

    #region cache

    public virtual string CustomerCodeCache { get; set; } = string.Empty;

    #endregion
}
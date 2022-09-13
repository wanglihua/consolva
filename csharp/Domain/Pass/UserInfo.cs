using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace shunda.Domain.Pass;

/// <summary>
/// 用户
/// </summary>
public class UserInfo : Entity
{
    public static readonly int DevUserId = 99999999; // 8 个 9

    public static readonly string DevLoginName = "dev";

    public static readonly int AdminUserId = 1;

    /// <summary>
    /// 登录名，非空
    /// </summary>
    [Required(ErrorMessage = "登录名不能为空！")]
    [MaxLength(100, ErrorMessage = "登录名长度不能大于100！")]
    [JsonProperty("loginName")]
    public virtual string LoginName { get; set; } = string.Empty;

    /// <summary>
    /// 用户名，非空
    /// </summary>
    [Required(ErrorMessage = "用户名不能为空！")]
    [MaxLength(100, ErrorMessage = "用户名长度不能大于100！")]
    [JsonProperty("userName")]
    public virtual string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码，非空
    /// </summary>
    [Required(ErrorMessage = "密码不能为空！")]
    [MaxLength(100, ErrorMessage = "密码长度不能大于100！")]
    [JsonProperty("password")]
    public virtual string Password { get; set; } = string.Empty;

    /// <summary>
    /// 登录时间，非空
    /// </summary>
    [Required(ErrorMessage = "登录时间不能为空！")]
    [JsonProperty("lastLoginTime")]
    public virtual DateTime LastLoginTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// 状态，非空
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(UserStatusModelBinder))]
    public virtual UserStatus Status { get; set; } = UserStatus.None;

}
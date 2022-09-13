using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using shunda.Core;

namespace shunda.Domain.Pass;

/// <summary>
/// 管理员
/// </summary>
public class AdminInfo : Entity
{
    /// <summary>
    /// 内置超级管理员的登录名
    /// </summary>
    public static readonly string SupperAdminLoginName = "admin";

    /// <summary>
    /// 用户
    /// </summary>
    [DisplayName("用户")]
    public virtual UserInfo User { get; set; } = new UserInfo();

    /// <summary>
    /// 登录名
    /// </summary>
    [DisplayName("登录名")]
    [Required(ErrorMessage = "登录名不能为空！")]
    [MaxLength(100, ErrorMessage = "登录名长度不能大于100！")]
    public virtual string LoginName { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    [DisplayName("用户名")]
    [Required(ErrorMessage = "用户名不能为空！")]
    [MaxLength(100, ErrorMessage = "用户名长度不能大于100！")]
    public virtual string UserName { get; set; } = string.Empty;
}
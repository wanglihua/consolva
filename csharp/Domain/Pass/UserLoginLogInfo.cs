using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace shunda.Domain.Pass;

/// <summary>
/// 用户登录日志
/// </summary>
public class UserLoginLogInfo
{
    public virtual int Id { get; set; }

    /// <summary>
    /// 登录用户
    /// </summary>
    [DisplayName("登录用户")]
    [Required(ErrorMessage = "登录用户不能为空！")]
    public virtual UserInfo User { get; set; } = new UserInfo();

    /// <summary>
    /// IP地址
    /// </summary>
    [DisplayName("IP地址")]
    [Required(ErrorMessage = "IP地址不能为空！")]
    [MaxLength(200, ErrorMessage = "IP地址长度不能大于200！")]
    public virtual string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// User Agent
    /// </summary>
    [DisplayName("User Agent")]
    [Required(ErrorMessage = "User Agent不能为空！")]
    [MaxLength(300, ErrorMessage = "User Agent长度不能大于300！")]
    public virtual string UserAgent { get; set; } = string.Empty;

    /// <summary>
    /// 登录时间
    /// </summary>
    [DisplayName("登录时间")]
    [Required(ErrorMessage = "登录时间不能为空！")]
    public virtual DateTime LoginTime { get; set; }
}
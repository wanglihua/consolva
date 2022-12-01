using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Pass;

namespace shunda.Admin.Models.Profile;

public class ProfileBasicModel
{
    /// <summary>
    ///     用户信息，不绑定，只用于显示信息
    /// </summary>
    [BindNever]
    [NotNull]
    public UserInfo? User { get; set; }

    /// <summary>
    ///     密码
    /// </summary>
    [MinLength(6, ErrorMessage = "密码长度不能小于6！")]
    [MaxLength(18, ErrorMessage = "密码长度不能大于18！")]
    // [Required(ErrorMessage = "密码不能为空！")]
    public string? Password { get; set; }

    /// <summary>
    ///     密码重输
    /// </summary>
    [MinLength(6, ErrorMessage = "密码重输长度不能小于6！")]
    [MaxLength(18, ErrorMessage = "密码重输长度不能大于18！")]
    // [Required(ErrorMessage = "密码重输不能为空！")]
    public string? PasswordAgain { get; set; }
}
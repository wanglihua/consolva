using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.Pass.Models.User;

public class UserDetailModel
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "登录名不能为空！")]
    [MaxLength(100, ErrorMessage = "登录名长度不能大于100！")]
    public string LoginName { get; set; } = string.Empty;

    [Required(ErrorMessage = "用户名不能为空！")]
    [MaxLength(100, ErrorMessage = "用户名长度不能大于100！")]
    public string UserName { get; set; } = string.Empty;

    [MinLength(6, ErrorMessage = "密码长度不能小于6！")]
    [MaxLength(18, ErrorMessage = "密码长度不能大于18！")]
    public string Password { get; set; } = string.Empty;
}
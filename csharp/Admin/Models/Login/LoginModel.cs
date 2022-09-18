using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Models.Login;

public class LoginModel
{
    [DisplayName("登录名")]
    [Required(ErrorMessage = "登录名不能为空！")]
    public string LoginName { get; set; } = string.Empty;

    [DisplayName("密码")]
    [Required(ErrorMessage = "密码不能为空！")]
    public string Password { get; set; } = string.Empty;

    [DisplayName("验证码")]
    [Required(ErrorMessage = "验证码不能为空！")]
    public string Captcha { get; set; } = string.Empty;
}
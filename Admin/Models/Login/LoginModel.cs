using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Models.Login;

public class LoginModel
{
    [Required(ErrorMessage = "登录名不能为空！")]
    public string LoginName { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空！")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "验证码不能为空！")]
    public string Captcha { get; set; } = string.Empty;
}
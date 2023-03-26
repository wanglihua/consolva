using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.App.Models.Login;

public class LoginModel {
    [Required(ErrorMessage = "登录名不能为空！")]
    [BindProperty(Name = "loginName")]
    public string LoginName { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空！")]
    [BindProperty(Name = "password")]
    public string Password { get; set; } = string.Empty;
}
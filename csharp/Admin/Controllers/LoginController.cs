using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Models.Login;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;

namespace shunda.Admin.Controllers;

[NoLoginRequired]
public class LoginController : Controller
{
    private readonly CaptchaService captchaService;
    private readonly LoginUserService loginUserService;

    public LoginController(LoginUserService loginUserService,
        CaptchaService captchaService)
    {
        this.loginUserService = loginUserService;
        this.captchaService = captchaService;
    }

    [HttpGet]
    [Route("/admin/login")]
    [NoLoginRequired]
    public IActionResult Index()
    {
        var model = new LoginModel();
        return View("LoginIndex", model);
    }

    [HttpPost]
    [Route("/admin/login")]
    [ValidateAntiForgeryToken]
    [NoLoginRequired]
    [Transactional]
    public IActionResult Login(LoginModel model)
    {
        if (ModelState.IsValid == false) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        if (captchaService.CheckInputCode(model.Captcha) == false)
        {
            // 如果发现验证码不对,应该马上把原来的验证码设为无效
            captchaService.MakeCurrentExpire();

            throw new ValidationException("验证码输入有错误！");
        }

        var loginName = model.LoginName.Trim();
        var password = model.Password.Trim();

        var loginSuccess = loginUserService.Login(loginName, password, Request);
        if (loginSuccess == false)
        {
            // 如果发现用户名或者密码不对,应该马上把原来的验证码设为无效
            captchaService.MakeCurrentExpire();

            throw new ValidationException("用户不存在或密码不正确！");
        }

        return Json(new {
            success = true,
            message = "登录成功！"
        });
    }

    [HttpPost]
    [Route("/admin/logout")]
    [NoLoginRequired]
    public IActionResult Logout()
    {
        loginUserService.Logout(Request, Response);

        return Json(new {
            success = true,
            message = "注销成功！"
        });
    }
}
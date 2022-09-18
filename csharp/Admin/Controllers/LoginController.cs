using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Models.Login;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Service;

namespace shunda.Admin.Controllers;

[NoLoginRequired]
public class LoginController : Controller
{
    private LoginUserService loginUserService;
    private CaptchaService captchaService;

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
        // 域名重定向

        // https 重定向            

        /*
         域名被屏蔽暂时去掉这个重定向
#if !DEBUG
        // 做可能的重定向
        string requestUrl = Request.GetEncodedUrl();
        if (string.IsNullOrWhiteSpace(requestUrl) == false 
            && (requestUrl.ToUpper().IndexOf("http://hk.iepoch.store".ToUpper()) < 0 && requestUrl.ToUpper().IndexOf("https://hk.iepoch.store".ToUpper()) < 0))
        {
            if (Request.IsHttps == false)
            {
                return Redirect("http://hk.iepoch.store/admin/login");
            }
            else
            {
                return Redirect("https://hk.iepoch.store/admin/login");
            }
        }
#endif
        */

        LoginModel model = new LoginModel();
        return View("LoginIndex", model);
    }

    [HttpPost]
    [Route("/admin/login")]
    [ValidateAntiForgeryToken()]
    [NoLoginRequired]
    [Transactional]
    public IActionResult Login(LoginModel model)
    {
        if (this.ModelState.IsValid == false)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        if (this.captchaService.CheckInputCode(model.Captcha) == false)
        {
            // 如果发现验证码不对,应该马上把原来的验证码设为无效
            this.captchaService.MakeCurrentExpire();

            throw new ValidationException("验证码输入有错误！");
        }

        string loginName = model.LoginName.Trim();
        string password = model.Password.Trim();

        bool loginSuccess = this.loginUserService.Login(loginName, password, Request);
        if (loginSuccess == false)
        {
            // 如果发现用户名或者密码不对,应该马上把原来的验证码设为无效
            this.captchaService.MakeCurrentExpire();

            throw new ValidationException("用户不存在或密码不正确！");
        }

        return Json(new {
            success = true,
            message = "登录成功！",
        });
    }

    [HttpPost]
    [Route("/admin/logout")]
    [NoLoginRequired]
    public IActionResult Logout()
    {
        int loginUserId = loginUserService.GetLoginUserId();

        this.loginUserService.Logout(this.Request, this.Response);

        return Json(new {
            success = true,
            message = "注销成功！",
        });
    }    
}
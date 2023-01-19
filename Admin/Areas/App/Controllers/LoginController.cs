using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.App.Models.Login;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.App.Controllers;

// todo 如果加入图形验证码，可以参考 shunda.Admin.Controllers.LoginController.Login() 的实现方式

[AppArea]
[NoLoginRequired]
public class LoginController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;

    public LoginController(ISession session, LoginUserService loginUserService) {
        this.session = session;
        this.loginUserService = loginUserService;
    }

    [HttpPost]
    [Route("/admin/app/login")]
    public IActionResult Login(LoginModel model) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(this.ModelState));
        }

        // todo 加上图形验证码

        var loginName = model.LoginName.Trim();
        var password = model.Password.Trim();

        var loginSuccess = loginUserService.Login(loginName, password, UserLoginLocation.App, Request);
        if (loginSuccess == false) {
            throw new ValidationException("用户不存在或密码不正确！");
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "登录成功！",
        });
    }
}
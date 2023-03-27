using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.App.Models.Login;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.App.Controllers;

[AppArea]
[NoLoginRequired]
public class LoginController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly UserLoginRateLimitService userLoginRateLimitService;

    public LoginController(ISession session,
        LoginUserService loginUserService,
        UserLoginRateLimitService userLoginRateLimitService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.userLoginRateLimitService = userLoginRateLimitService;
    }

    [HttpPost]
    [Route("/admin/app/login")]
    [Transactional]
    public IActionResult Login(LoginParam model) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(this.ModelState));
        }

        var loginName = model.LoginName.Trim();
        var password = model.Password.Trim();

        userLoginRateLimitService.CheckRateLimit(loginName);

        var loginResult = loginUserService.Login(loginName, password, UserLoginLocation.App, Request);
        if (loginResult.Item1 == false) {
            throw new ValidationException("用户不存在或密码不正确！");
        }

        UserInfo user = loginResult.Item2;

        return Json(new JsonResultData {
            Success = true,
            Message = "登录成功！",
            Data = new LoginModel {
                UserId = user.Id,
                UserName = user.LoginName,
            },
        });
    }
}
using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.App.Models.Login;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.App.Controllers;

[AppArea]
[NoLoginRequired]
public class LoginController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly UserLoginRateLimitService userLoginRateLimitService;
    private readonly PassResService passResService;

    public LoginController(ISession session,
        LoginUserService loginUserService,
        UserLoginRateLimitService userLoginRateLimitService,
        PassResService passResService) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.userLoginRateLimitService = userLoginRateLimitService;
        this.passResService = passResService;
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

        int userId = user.Id;
        bool canExpressQuery = passResService.UserHasPassRes(userId, PassRes.AdminApp.ExpressQuery);
        bool canExpressOnShelf = passResService.UserHasPassRes(userId, PassRes.AdminApp.ExpressOnShelf);
        bool canExpressWarehouseCheck = passResService.UserHasPassRes(userId, PassRes.AdminApp.ExpressWarehouseCheck);
        bool canExpressOffShelf = passResService.UserHasPassRes(userId, PassRes.AdminApp.ExpressOffShelf);
        bool canExpressReturnOffShelf = passResService.UserHasPassRes(userId, PassRes.AdminApp.ExpressReturnOffShelf);
        bool canExpressDump = passResService.UserHasPassRes(userId, PassRes.AdminApp.ExpressDump);
        bool canExpressPacking = passResService.UserHasPassRes(userId, PassRes.AdminApp.ExpressPacking);
        bool canPackBoxQuery = passResService.UserHasPassRes(userId, PassRes.AdminApp.PackBoxQuery);
        bool canPackBoxShip = passResService.UserHasPassRes(userId, PassRes.AdminApp.PackBoxShip);

        bool canPhotoSignIn = passResService.UserHasPassRes(userId, PassRes.AdminApp.PhotoSignIn);
        bool canPhotoCheckIn = passResService.UserHasPassRes(userId, PassRes.AdminApp.PhotoCheckIn);
        bool canPhotoIssue = passResService.UserHasPassRes(userId, PassRes.AdminApp.PhotoIssue);
        bool canPhotoPack = passResService.UserHasPassRes(userId, PassRes.AdminApp.PhotoPack);
        bool canPhotoReturn = passResService.UserHasPassRes(userId, PassRes.AdminApp.PhotoReturn);
        bool canPhotoShip = passResService.UserHasPassRes(userId, PassRes.AdminApp.PhotoShip);
        bool canPhoto = canPhotoSignIn || canPhotoCheckIn || canPhotoIssue || canPhotoPack || canPhotoReturn || canPhotoShip;

        return Json(new JsonResultData {
            Success = true,
            Message = "登录成功！",
            Data = new LoginModel {
                UserId = user.Id,
                UserName = user.LoginName,
                CanExpressQuery = canExpressQuery,
                CanExpressOnShelf = canExpressOnShelf,
                CanExpressWarehouseCheck = canExpressWarehouseCheck,
                CanExpressOffShelf = canExpressOffShelf,
                CanExpressReturnOffShelf = canExpressReturnOffShelf,
                CanExpressDump = canExpressDump,
                CanExpressPacking = canExpressPacking,
                CanPackBoxQuery = canPackBoxQuery,
                CanPackBoxShip = canPackBoxShip,
                CanPhotoSignIn = canPhotoSignIn,
                CanPhotoCheckIn = canPhotoCheckIn,
                CanPhotoIssue = canPhotoIssue,
                CanPhotoPack = canPhotoPack,
                CanPhotoReturn = canPhotoReturn,
                CanPhotoShip = canPhotoShip,
                CanPhoto = canPhoto,
            },
        });
    }
}
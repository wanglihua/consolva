using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Models.AdminLoginUser;
using shunda.Admin.Services;
using shunda.Domain.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Components;

public class AdminLoginUserViewComponent : ViewComponent
{
    private readonly LoginUserService loginUserService;
    private readonly ISession session;

    public AdminLoginUserViewComponent(ISession session, LoginUserService loginUserService)
    {
        this.session = session;
        this.loginUserService = loginUserService;
    }

    public IViewComponentResult Invoke()
    {
        var model = new AdminLoginUserModel();

        var loginUserId = loginUserService.GetLoginUserId();
        var user = session.Get<UserInfo>(loginUserId); // 这里采用查询数据的方式是为了应对 dev user

        if (user == null)
        {
            if (loginUserId == UserInfo.DevUserId) {
                user = new UserInfo {
                    Id = UserInfo.DevUserId,
                    LoginName = UserInfo.DevLoginName,
                    UserName = UserInfo.DevLoginName
                };
            } else {
                user = new UserInfo();
            }
        }

        model.LoginName = user.LoginName;
        model.UserName = user.UserName;

        return View("AdminLoginUser", model);
    }
}
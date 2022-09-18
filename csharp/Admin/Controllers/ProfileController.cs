
using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Models.Profile;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Pass;
using shunda.Service;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Controllers;

[AdminAuth]
public class ProfileController : Controller
{
    private ISession session;
    private LoginUserService loginUserService;
    private PasswordService passwordService;

    public ProfileController(ISession session,
        LoginUserService loginUserService,
        PasswordService passwordService)
    {
        this.session = session;
        this.loginUserService = loginUserService;
        this.passwordService = passwordService;
    }

    [HttpGet]
    [Route("/admin/profile/basic")]
    public IActionResult Basic()
    {
        int loginUserId = loginUserService.GetLoginUserId();
        UserInfo user = session.Get<UserInfo>(loginUserId);
        if (user == null)
        {
            return Content("用户未登录，或者指定的用户不存在！");
        }

        ProfileBasicModel model = new ProfileBasicModel();
        model.User = user;

        return View("ProfileBasic", model);
    }

    [HttpPost]
    [Route("/admin/profile/basic/save")]
    [Transactional]
    public IActionResult BasicSave(ProfileBasicModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        if (string.IsNullOrWhiteSpace(model.Password) == false)
        {
            if (model.Password != model.PasswordAgain)
            {
                throw new ValidationException("密码重输和密码应该一致！");
            }

            int loginUserId = loginUserService.GetLoginUserId();
            UserInfo userInDb = session.Get<UserInfo>(loginUserId);

            userInDb.Password = passwordService.Encrypt(model.Password);

            session.Update(userInDb);
            session.Flush();
        }

        return Json(new {
            success = true,
            message = "保存成功！",
        });
    }
}
using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Models.Profile;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Pass;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Controllers;

[AdminAuth]
public class ProfileController : Controller {
    private readonly LoginUserService loginUserService;
    private readonly PasswordService passwordService;
    private readonly ISession session;

    public ProfileController(ISession session,
        LoginUserService loginUserService,
        PasswordService passwordService) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.passwordService = passwordService;
    }

    [HttpGet]
    [Route("/admin/profile/basic")]
    public IActionResult Basic() {
        var user = loginUserService.GetLoginUser();

        var model = new ProfileBasicModel();
        model.User = user;

        return View("ProfileBasic", model);
    }

    [HttpPost]
    [Route("/admin/profile/basic/save")]
    [Transactional]
    public IActionResult BasicSave(ProfileBasicModel model) {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        if (string.IsNullOrWhiteSpace(model.Password) == false) {
            if (model.Password != model.PasswordAgain) {
                throw new ValidationException("密码重输和密码应该一致！");
            }

            var userInDb = loginUserService.GetLoginUser();

            userInDb.Password = passwordService.Encrypt(model.Password);

            session.Update(userInDb);
            session.Flush();
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！"
        });
    }
}
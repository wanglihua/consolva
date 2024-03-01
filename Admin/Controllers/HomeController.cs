using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Admin.Models.Home;
using shunda.Admin.Services;
using shunda.Core.Web;
using shunda.Service;

namespace shunda.Admin.Controllers;

public class HomeController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly SidebarStateService sidebarStateService;
    private readonly LoginUserService loginUserService;

    public HomeController(SidebarStateService sidebarStateService,
        LoginUserService loginUserService) {
        this.sidebarStateService = sidebarStateService;
        this.loginUserService = loginUserService;
    }

    [NoLoginRequired]
    [HttpGet]
    [Route("/")]
    public IActionResult Index() {
#if DEBUG
        return RedirectToAction("AdminIndex", "Home");
#else
        return View("HomeIndex");
#endif
    }

    [AdminAuth]
    [HttpGet]
    [Route("/admin")]
    public IActionResult AdminIndex() {
        var model = new HomeAdminIndexModel();

        return View("HomeAdminIndex", model);
    }

    [AdminAuth]
    [HttpPost]
    [Route("/admin/home/set/sidebar/state")]
    public IActionResult SetSidebarState(bool isMin) {
        int loginUserId = loginUserService.GetLoginUserId();
        sidebarStateService.SetState(loginUserId, isMin);

        return Json(new Dictionary<string, object> {
            { "success", true }, 
            { "message", "Sidebar状态设置成功！" }
        });
    }
}

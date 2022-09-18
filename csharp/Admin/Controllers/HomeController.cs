using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Admin.Models.Home;
using shunda.Core.Web;
using shunda.Service;

namespace shunda.Admin.Controllers;

public class HomeController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private SidebarStateService sidebarStateService;
    
    public HomeController(SidebarStateService sidebarStateService)
    {
        this.sidebarStateService = sidebarStateService;
    }

    [NoLoginRequired]
    [HttpGet]
    [Route("/")]
    public IActionResult Index()
    {
        return RedirectToAction("AdminIndex", "Home");
    }

    [HttpGet]
    [Route("/admin")]
    public IActionResult AdminIndex()
    {
        HomeAdminIndexModel model = new HomeAdminIndexModel();

        return View("HomeAdminIndex", model);
    }

    [HttpPost]
    [Route("/admin/home/set/sidebar/state")]
    public IActionResult SetSidebarState(bool isMin)
    {
        sidebarStateService.SetState(isMin);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "Sidebar状态设置成功！" }
        });
    }    
}
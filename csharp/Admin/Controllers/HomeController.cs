using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Admin.Models.Home;
using shunda.Core.Web;

namespace shunda.Admin.Controllers;

public class HomeController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    public HomeController()
    {
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
}
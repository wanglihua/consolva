using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Models.Home;
using shunda.Core.Web;

namespace shunda.Client.Controllers;

public class HomeController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    [NoLoginRequired]
    [HttpGet]
    [Route("/")]
    public IActionResult Index()
    {
        return RedirectToAction("ClientIndex", "Home");
    }

    [HttpGet]
    [Route("/client")]
    public IActionResult ClientIndex()
    {
        HomeClientIndexModel model = new HomeClientIndexModel();
        return View("HomeClientIndex", model);
    }
}
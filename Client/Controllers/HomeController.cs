using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Models.Home;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

public class HomeController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public HomeController(ISession session) {
        this.session = session;
    }

    // 这个只能通过dotnet 部署的端口号访问到
    [NoLoginRequired]
    [HttpGet]
    [Route("/")]
    public IActionResult Index() {
        var model = new HomeIndexModel();
        return View("HomeIndex", model);
    }

    // 这个可以通过nginx反向代理访问到
    [HttpGet]
    [Route("/client")]
    public IActionResult ClientIndex() {
        var model = new HomeClientIndexModel();
        return View("HomeClientIndex", model);
    }
}
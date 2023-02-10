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

    [NoLoginRequired]
    [HttpGet]
    [Route("/")]
    public IActionResult Index() {
        HomeIndexModel model = new HomeIndexModel();
        return View("HomeIndex", model);
    }
}
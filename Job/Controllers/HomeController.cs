using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core.Web;
using shunda.Job.Models.Home;
using ISession = NHibernate.ISession;

namespace shunda.Job.Controllers;

public class HomeController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();
    
    private readonly ISession session;

    public HomeController(ISession session) {
        this.session = session;
    }

    [NoLoginRequired]
    [HttpGet]
    [Route("/")]
    [Route("/job")]
    public IActionResult Index() {
        HomeIndexModel model = new HomeIndexModel();
        return View("HomeIndex", model);
    }
}
using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Job.Controllers;

public class HomeController : Controller
{
    private ISession session;

    public HomeController(ISession session)
    {
        this.session = session;     
    }

    [HttpGet]
    [Route("/")]
    public IActionResult Index()
    {
        return Json(true);
    }
}
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace shunda.Admin.Controllers;

public class FaviconController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    [Route("/favicon.ico")]
    public IActionResult Index()
    {
        return File("", "");
    }
}
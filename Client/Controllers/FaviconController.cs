using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core;
using shunda.Core.Web;

namespace shunda.Client.Controllers;

[NoLoginRequired]
public class FaviconController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    [Route("/favicon.ico")]
    public IActionResult Index() {
        return File("/static/client/images/favicon.png", ContentType.ImagePng);
    }
}
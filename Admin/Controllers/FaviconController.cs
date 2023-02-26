using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core;
using shunda.Core.Web;

namespace shunda.Admin.Controllers;

[NoLoginRequired]
public class FaviconController : Controller
{
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    // private IWebHostEnvironment webHostEnvironment;
    //
    // public FaviconController(IWebHostEnvironment webHostEnvironment)
    // {
    //     this.webHostEnvironment = webHostEnvironment;
    // }

    [Route("/favicon.ico")]
    public IActionResult Index()
    {
        // string faviconPngFullPath = Path.Combine(webHostEnvironment.WebRootPath, "static", "images", "favicon.png");
        // return PhysicalFile(faviconPngFullPath, ContentType.ImagePng);
        return File("/static/images/favicon.png", ContentType.ImagePng);
    }
}
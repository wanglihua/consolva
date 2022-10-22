using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core;

namespace shunda.Admin.Controllers;

public class FaviconController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    // private IWebHostEnvironment webHostEnvironment;
    //
    // public FaviconController(IWebHostEnvironment webHostEnvironment)
    // {
    //     this.webHostEnvironment = webHostEnvironment;
    // }

    [Route("/favicon.ico")]
    public IActionResult Index()
    {
        // string faviconPngFullPath = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot", "static", "images", "favicon.png");
        // return PhysicalFile(faviconPngFullPath, ContentType.ImagePng);
        return File("/static/images/favicon.png", ContentType.ImagePng);
    }
}
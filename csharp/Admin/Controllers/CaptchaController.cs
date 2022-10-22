using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Core.Web;

namespace shunda.Admin.Controllers;

[NoLoginRequired]
public class CaptchaController : Controller
{
    private CaptchaService captchaService;

    public CaptchaController(CaptchaService captchaService)
    {
        this.captchaService = captchaService;
    }

    [HttpGet]
    [Route("/admin/captcha")]
    public IActionResult Index()
    {
        MemoryStream memoryStream = captchaService.Create();
        byte[] bytes = memoryStream.GetBuffer();
        memoryStream.Close();

        Response.Body.Dispose();
        return File(bytes, ContentType.ImagePng);
    }
}
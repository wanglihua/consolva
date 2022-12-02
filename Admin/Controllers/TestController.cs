using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using shunda.Service.Wechat.WechatApp;

namespace shunda.Admin.Controllers;

[NoLoginRequired]
public class TestController : Controller
{
    private readonly WechatAppAccessTokenService wechatAppAccessTokenService;

    public TestController(WechatAppAccessTokenService wechatAppAccessTokenService)
    {
        this.wechatAppAccessTokenService = wechatAppAccessTokenService;
    }

    [HttpGet]
    [Route("/admin/test")]
    public IActionResult Index()
    {
        string token = wechatAppAccessTokenService.GetAccessToken();
        return Content(token);
    }
}
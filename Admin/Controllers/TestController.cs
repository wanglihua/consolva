using System.Globalization;
using CSRedis;
using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core.Redis;
using shunda.Core.Web;
using shunda.Service.Wechat.WechatApp;

namespace shunda.Admin.Controllers;

[NoLoginRequired]
public class TestController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

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
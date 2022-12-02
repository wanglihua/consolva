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
        // string token = wechatAppAccessTokenService.GetAccessToken();

        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();
        string token = redisClient.Get("token");

        log.Trace($"token: {token}");
        log.Trace($"token is null: {token == null}");

        return Content(token ?? string.Empty);
    }
}
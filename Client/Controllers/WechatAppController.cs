using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core.Web;
using shunda.Service.Wechat.WechatApp;

namespace shunda.Client.Controllers;

[NoLoginRequired]
public class WechatAppController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly WechatAppPhoneNumberService wechatAppPhoneNumberService;

    public WechatAppController(WechatAppPhoneNumberService wechatAppPhoneNumberService)
    {
        this.wechatAppPhoneNumberService = wechatAppPhoneNumberService;
    }

    [HttpPost]
    [Route("/client/wechat/app/get/phone/number")]
    public IActionResult GetPhoneNumber(string code)
    {
        string phoneNumber = wechatAppPhoneNumberService.GetPhoneNumber(code);

        log.Trace($"phone number: {phoneNumber}");

        return Json(new {
            success = true,
            message = "操作成功！",
            phoneNumber = phoneNumber,
        });
    }
}
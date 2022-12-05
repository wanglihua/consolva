using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using shunda.Client.Services;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Service.Wechat.WechatApp;

namespace shunda.Client.Controllers;

[NoLoginRequired]
public class WechatAppController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly WechatAppAuthCodeSessionService wechatAppAuthCodeSessionService;
    private readonly LoginCustomerService loginCustomerService;
    private readonly WechatAppPhoneNumberService wechatAppPhoneNumberService;

    public WechatAppController(
        WechatAppAuthCodeSessionService wechatAppAuthCodeSessionService,
        LoginCustomerService loginCustomerService,
        WechatAppPhoneNumberService wechatAppPhoneNumberService)
    {
        this.wechatAppAuthCodeSessionService = wechatAppAuthCodeSessionService;
        this.loginCustomerService = loginCustomerService;
        this.wechatAppPhoneNumberService = wechatAppPhoneNumberService;
    }

    public class LoginParam
    {
        [BindProperty(Name = "code")]
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;
    }

    [HttpPost]
    [Route("/client/wechat/app/login")]
    public IActionResult Login([FromBody] LoginParam param)
    {
        if (ModelState.IsValid == false)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        var result = wechatAppAuthCodeSessionService.GetResult(param.Code);

        return Json(new {
            success = true,
            message = "登录成功！",
        });
    }

    public class GetPhoneNumberParam
    {
        [BindProperty(Name = "code")]
        [JsonProperty("code")]
        [Required(ErrorMessage = "获取电话号码, code 不能为空！")]
        public string Code { get; set; } = string.Empty;
    }

    [HttpPost]
    [Route("/client/wechat/app/get/phone/number")]
    public IActionResult GetPhoneNumber([FromBody] GetPhoneNumberParam param)
    {
        if (ModelState.IsValid == false)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        string code = param.Code.Trim();
        string phoneNumber = wechatAppPhoneNumberService.GetPhoneNumber(code);

        log.Trace($"phone number: {phoneNumber}");

        return Json(new {
            success = true,
            message = "操作成功！",
            phoneNumber = phoneNumber,
        });
    }
}
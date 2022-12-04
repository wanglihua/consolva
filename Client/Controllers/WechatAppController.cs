using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using shunda.Core.Util;
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
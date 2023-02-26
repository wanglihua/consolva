using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Models.WechatAppLogin;
using shunda.Client.Services;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Service.Wechat;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

[NoLoginRequired]
public class WechatAppLoginController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly WechatAppLoginService wechatAppLoginService;
    private readonly WechatAppPhoneNumberService wechatAppPhoneNumberService;


    public WechatAppLoginController(
        ISession session,
        WechatAppLoginService wechatAppLoginService,
        WechatAppPhoneNumberService wechatAppPhoneNumberService
    ) {
        this.session = session;
        this.wechatAppLoginService = wechatAppLoginService;
        this.wechatAppPhoneNumberService = wechatAppPhoneNumberService;
    }
    

    [HttpPost]
    [Route("/client/wechat/app/login")]
    // [Transactional]  WechatAppLoginService.Login() 里面有事务还有互斥锁，这里事务不能加
    public IActionResult Login([FromBody] WechatAppLoginParam param) {
        ValidationUtil.Validate<WechatAppLoginParamValidator, WechatAppLoginParam>(param);

        wechatAppLoginService.Login(param.Code, Request);

        return Json(new JsonResultData {
            Success = true,
            Message = "登录成功！",
        });
    }

    /* 这部分现在暂时也不用
    public class GetPhoneNumberParam {
        [BindProperty(Name = "code")]
        [JsonProperty("code")]
        [Required(ErrorMessage = "获取电话号码, code 不能为空！")]
        public string Code { get; set; } = string.Empty;
    }

    [HttpPost]
    [Route("/client/wechat/app/get/phone/number")]
    public IActionResult GetPhoneNumber([FromBody] GetPhoneNumberParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        string code = param.Code.Trim();
        string phoneNumber = wechatAppPhoneNumberService.GetPhoneNumber(code);

        log.Trace($"phone number: {phoneNumber}");

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new {
                phoneNumber = phoneNumber,
            }
        });
    }
    */
}
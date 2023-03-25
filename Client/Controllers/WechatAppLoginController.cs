using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Models.WechatAppLogin;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Client;
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

        LoginResult loginResult = wechatAppLoginService.Login(param.Code, Request);
        if (loginResult.ResultCode != LoginResultCode.Success || loginResult.Customer == null) {
            throw new Exception("登陆出现异常，请重试！");
        }

        var customer = loginResult.Customer;
        if (customer.Agreed == false) {
            if (param.Agreed == false) {
                return Json(new JsonResultData {
                    Success = true,
                    Message = "登录失败，请同意用户协议和隐私协议，再继续！",
                    Data = new WechatAppLoginModel {
                        Agreed = false,
                    }
                });
            }

            if (param.Agreed == true) {
                Transactional.Wrap(session, () => {
                    customer.Agreed = true;
                    session.Update(customer);
                    session.Flush();
                });
            }
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "登录成功！",
            Data = new WechatAppLoginModel {
                Agreed = true,
            }
        });
    }

    [HttpPost]
    [Route("/client/wechat/app/show/login")]
    public IActionResult ShowLogin() {
        var clientInstructTypeList = new List<ClientInstructionType> {
            ClientInstructionType.ServiceAgreement,
            ClientInstructionType.PrivacyStatement,
        };

        var clientInstructList = session.Query<ClientInstructionInfo>()
            .Where(e => clientInstructTypeList.Contains(e.Type))
            .ToList();

        var serviceAgreement = clientInstructList
            .Where(e => e.Type == ClientInstructionType.ServiceAgreement)
            .Take(1).SingleOrDefault();
        string serviceAgreementUrl = serviceAgreement?.Url ?? string.Empty;

        var privacyStatement = clientInstructList
            .Where(e => e.Type == ClientInstructionType.PrivacyStatement)
            .Take(1).SingleOrDefault();
        string privacyStatementUrl = privacyStatement?.Url ?? string.Empty;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new WechatAppLoginShowLoginModel {
                ServiceAgreementUrl = serviceAgreementUrl,
                PrivacyStatementUrl = privacyStatementUrl,
            },
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
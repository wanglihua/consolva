using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHibernate.Criterion;
using NLog;
using shunda.Client.Services;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Service.Customer;
using shunda.Service.Wechat;
using ISession = NHibernate.ISession;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace shunda.Client.Controllers;

[NoLoginRequired]
public class WechatAppController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly WechatAppAuthCodeSessionService wechatAppAuthCodeSessionService;
    private readonly CustomerService customerService;
    private readonly LoginCustomerService loginCustomerService;
    private readonly WechatAppPhoneNumberService wechatAppPhoneNumberService;
    private readonly WechatOfficialAccountFollowerService wechatOfficialAccountFollowerService;

    public WechatAppController(
        ISession session,
        WechatAppAuthCodeSessionService wechatAppAuthCodeSessionService,
        CustomerService customerService,
        LoginCustomerService loginCustomerService,
        WechatAppPhoneNumberService wechatAppPhoneNumberService,
        WechatOfficialAccountFollowerService wechatOfficialAccountFollowerService) {
        this.session = session;
        this.wechatAppAuthCodeSessionService = wechatAppAuthCodeSessionService;
        this.customerService = customerService;
        this.loginCustomerService = loginCustomerService;
        this.wechatAppPhoneNumberService = wechatAppPhoneNumberService;
        this.wechatOfficialAccountFollowerService = wechatOfficialAccountFollowerService;
    }

    public class LoginParam {
        [BindProperty(Name = "code")]
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;
    }

    public class LoginParamValidator : AbstractValidator<LoginParam> {
        public LoginParamValidator() {
            RuleFor(e => e.Code).NotNull().NotEmpty().WithMessage("code不能为空！");
        }
    }

    [HttpPost]
    [Route("/client/wechat/app/login")]
    [Transactional]
    public IActionResult Login([FromBody] LoginParam param) {
        ValidationUtil.Validate<LoginParamValidator, LoginParam>(param);

        var result = wechatAppAuthCodeSessionService.GetResult(param.Code);
        string wechatAppOpenId = result.OpenId;
        string wechatUnionId = result.UnionId;

        ICriterion criterion = Restrictions.Where<CustomerInfo>(e => e.WechatAppOpenId == wechatAppOpenId);
        if (string.IsNullOrWhiteSpace(wechatUnionId) == false) // wechatUnionId 非空时才能带上这个查询 
        {
            criterion = Restrictions.Or(criterion, Restrictions.Where<CustomerInfo>(e => e.WechatUnionId == wechatUnionId));
        }

        var customer = session.QueryOver<CustomerInfo>()
            .Where(criterion)
            .Take(1)
            .SingleOrDefault();

        if (customer != null && (customer.WechatAppOpenId != wechatAppOpenId || customer.WechatUnionId != wechatUnionId)) {
            customer.WechatAppOpenId = wechatAppOpenId;
            customer.WechatUnionId = wechatUnionId;

            // 可能的更新微信公众号 open id
            if (string.IsNullOrWhiteSpace(customer.WechatOfficialAccountOpenId)) {
                string wechatOfficialAccountOpenId = wechatOfficialAccountFollowerService.GetWechatOfficialAccountOpenId(wechatUnionId);
                if (string.IsNullOrWhiteSpace(wechatOfficialAccountOpenId) == false) {
                    customer.WechatOfficialAccountOpenId = wechatOfficialAccountOpenId;
                }
            }

            session.Update(customer);
            session.Flush();
        }

        if (customer == null) {
            string wechatOfficialAccountOpenId = wechatOfficialAccountFollowerService.GetWechatOfficialAccountOpenId(wechatUnionId); // 这个结果可能为空
            customer = customerService.CreateCustomerByOpenId(wechatAppOpenId, wechatOfficialAccountOpenId, wechatUnionId);
        }

        loginCustomerService.Login(customer, CustomerLoginStage.WechatAppLogin, Request);

        return Json(new JsonResultData {
            Success = true,
            Message = "登录成功！",
        });
    }

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
}
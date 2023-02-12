using Microsoft.AspNetCore.Mvc;
using shunda.Client.Models.Login;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Service.Customer;

namespace shunda.Client.Controllers;

[NoLoginRequired]
public class LoginController : Controller
{
    private readonly CaptchaService captchaService;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CustomerCodeService customerCodeService;

    public LoginController(
        CaptchaService captchaService,
        LoginCustomerService loginCustomerService,
        CustomerCodeService customerCodeService)
    {
        this.captchaService = captchaService;
        this.loginCustomerService = loginCustomerService;
        this.customerCodeService = customerCodeService;
    }
    
    // todo 加上一个登录页面

    [HttpPost]
    [Route("/client/login")]
    [Transactional]
    public IActionResult Login(LoginModel model)
    {
        if (ModelState.IsValid == false)
        {
            // if (model.IsReactRequest)
            // {
            //     throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(this.ModelState));
            // }
            // else
            // {
            //     throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
            // }

            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(this.ModelState));
        }

        if (captchaService.CheckInputCode(model.Captcha) == false)
        {
            // 如果发现验证码不对,应该马上把原来的验证码设为无效
            captchaService.MakeCurrentExpire();

            throw new ValidationException("验证码输入有错误！");
        }

        string loginName = model.LoginName.Trim();
        string password = model.Password.Trim();

        LoginResult loginResult = loginCustomerService.Login(loginName, password, Request);

        if (loginResult.ResultCode == LoginResultCode.CustomerFrozen)
        {
            throw new ValidationException("用户账号已冻结！");
        }

        if (loginResult.ResultCode != LoginResultCode.Success)
        {
            // 如果发现用户名或者密码不对,应该马上把原来的验证码设为无效
            captchaService.MakeCurrentExpire();

            throw new ValidationException("用户不存在或密码不正确！");
        }

        string customerCode = string.Empty;
        string vip = string.Empty;

        CustomerInfo? customer = loginResult.Customer;
        if (customer != null)
        {
            customerCode = customerCodeService.GetCustomerCode(customer.Id);

            vip = customer.Level.Name;
            if (customer.Level == CustomerLevel.Vip)
            {
                vip = customer.VipRank.Name;
            }
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "登录成功！" },
            { "customerCode", customerCode },
            { "vip", vip },
        });
    }

    [HttpPost]
    [Route("/client/logout")]
    public IActionResult Logout()
    {
        loginCustomerService.Logout(Request, Response);

        return Json(new JsonResultData {
            Success = true,
            Message = "注销成功！"
        });
    }
}
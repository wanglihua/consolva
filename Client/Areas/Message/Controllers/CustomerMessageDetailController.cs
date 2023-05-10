using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Message.Models.CustomerMessageDetail;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Message;
using shunda.Service.Customer;
using shunda.Service.Message;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Message.Controllers;

[MessageArea]
public class CustomerMessageDetailController : Controller {
    private readonly ISession session;
    private readonly WechatAppLoginService wechatAppLoginService;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CustomerCodeService customerCodeService;
    private readonly CustomerMessageClientUrlService customerMessageClientUrlService;

    public CustomerMessageDetailController(
        ISession session,
        WechatAppLoginService wechatAppLoginService,
        LoginCustomerService loginCustomerService,
        CustomerCodeService customerCodeService,
        CustomerMessageClientUrlService customerMessageClientUrlService
    ) {
        this.session = session;
        this.wechatAppLoginService = wechatAppLoginService;
        this.loginCustomerService = loginCustomerService;
        this.customerCodeService = customerCodeService;
        this.customerMessageClientUrlService = customerMessageClientUrlService;
    }

    [HttpPost]
    [Route("/client/customer/message/detail")]
    public IActionResult Detail([FromBody] CustomerMessageDetailParam param) {
        ValidationUtil.Validate<CustomerMessageDetailParamValidator, CustomerMessageDetailParam>(param);

        // 后面可以把这个判断去掉
        if (string.IsNullOrWhiteSpace(param.Code)) {
            LoginResult loginResult = wechatAppLoginService.Login(param.Code, Request);
            if (loginResult.ResultCode != LoginResultCode.Success || loginResult.Customer == null) {
                throw new Exception("登陆出现异常，请重试！");
            }
        }

        int messageId = param.MessageId;
        var customerMessage = session.Get<CustomerMessageInfo>(messageId);
        if (customerMessage == null) {
            throw new ValidationException($"指定的客户消息#{messageId}不存在！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        if (customerMessage.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的客户消息#{messageId}不属于当前登录客户！");
        }

        string type = customerMessage.Type.Name;
        string customerCode = customerCodeService.GetCustomerCode(customerMessage.Customer.Id);
        string orderCode = customerMessage.Order?.Code ?? string.Empty;
        string sendTime = customerMessage.SendTime.ToString("yyyy-MM-dd HH:mm");
        string content = customerMessage.Content;
        (string url, string urlTitle) = customerMessageClientUrlService.GetUrl(customerMessage);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CustomerMessageDetailModel {
                Type = type,
                CustomerCode = customerCode,
                OrderCode = orderCode,
                SendTime = sendTime,
                Content = content,
                Url = url,
                UrlTitle = urlTitle,
            },
        });
    }
}
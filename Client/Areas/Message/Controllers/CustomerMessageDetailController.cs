using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Message.Models.CustomerMessageDetail;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Message;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Message.Controllers;

[MessageArea]
public class CustomerMessageDetailController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public CustomerMessageDetailController(
        ISession session,
        LoginCustomerService loginCustomerService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/customer/message/detail")]
    public IActionResult Detail([FromBody] CustomerMessageDetailParam param) {
        ValidationUtil.Validate<CustomerMessageDetailParamValidator, CustomerMessageDetailParam>(param);

        int messageId = param.MessageId;
        var customerMessage = session.Get<CustomerMessageInfo>(messageId);
        if (customerMessage == null) {
            throw new ValidationException($"指定的客户消息#{messageId}不存在！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        if (customerMessage.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的客户消息#{messageId}不属于当前登录客户！");
        }


        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}
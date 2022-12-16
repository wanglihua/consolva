using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Message.Models.CustomerMessage;
using shunda.Client.Services;
using shunda.Core.Web;
using shunda.Domain.Message;
using shunda.Service.Message;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Message.Controllers;

public class CustomerMessageController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CustomerMessageClientViewRecordService customerMessageClientViewRecordService;

    public CustomerMessageController(ISession session,
        LoginCustomerService loginCustomerService,
        CustomerMessageClientViewRecordService customerMessageClientViewRecordService)
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.customerMessageClientViewRecordService = customerMessageClientViewRecordService;
    }

    [HttpPost]
    [Route("/client/customer/message/list")]
    public IActionResult List([FromBody] CustomerMessageListParam param)
    {
        // customerMessageClientViewRecordService

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CustomerMessageListModel {
            },
        });
    }

    [NonAction]
    private IQueryable<CustomerMessageInfo> GetListQuery(CustomerMessageListParam param)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        return session.Query<CustomerMessageInfo>()
            .Where(e => e.Customer.Id == loginCustomerId);
    }
}
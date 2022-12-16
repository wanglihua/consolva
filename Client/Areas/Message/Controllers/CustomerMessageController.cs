using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Message.Models.CustomerMessage;
using shunda.Client.Models;
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
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var listQuery = GetListQuery(param);

        if (param.LastId != ListLastIdModel.LAST_ID_NOT_SET_VALUE)
        {
            // 跟降序排序匹配
            int lastIdQuery = param.LastId;
            listQuery = listQuery.Where(e => e.Id < lastIdQuery); // 降序排序id大的会被先取得
        }


        var msgList = listQuery
            .OrderByDescending(e => e.Id)
            .Take(15)
            .ToList();

        var msgItemList = msgList.Select(e => new CustomerMessageListItem {
            Id = e.Id,
            Type = e.Type.Name,
            Time = e.SendTime.ToString("yyyy-MM-dd HH:mm"),
            Content = e.Content,
        }).ToList();

        var lastData = msgList.LastOrDefault();
        int lastId = lastData?.Id ?? ListLastIdModel.LAST_ID_NO_MORE_DATA_VALUE;

        int msgCount = GetListQuery(param).Count();

        if (param.LastId == ListLastIdModel.LAST_ID_NOT_SET_VALUE) // 只需要这个时候设置就可以了，就是界面打开第一次加载列表的时候
        {
            customerMessageClientViewRecordService.RecordView(loginCustomerId, msgList.FirstOrDefault()?.Id ?? 0);
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CustomerMessageListModel {
                MessageList = msgItemList,
                MessageCount = msgCount,
                LastId = lastId,
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
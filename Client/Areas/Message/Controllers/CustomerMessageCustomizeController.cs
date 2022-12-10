using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Message.Models.CustomerMessageCustomize;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Message;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Message.Controllers;

[MessageArea]
public class CustomerMessageCustomizeController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public CustomerMessageCustomizeController(ISession session, LoginCustomerService loginCustomerService)
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/customer/message/customize/list")]
    public IActionResult List([FromBody] CustomerMessageCustomizeListParam param)
    {
        IList<CustomerMessageCustomizeListItem> messageCustomizeList = GetMessageCustomizeList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CustomerMessageCustomizeListModel() {
                MessageCustomizeList = messageCustomizeList,
            }
        });
    }

    [NonAction]
    private IList<CustomerMessageCustomizeListItem> GetMessageCustomizeList()
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var msgCustomizeList = session.Query<CustomerMessageClientCustomizeInfo>()
            .Where(e => e.CustomerId == loginCustomerId)
            .ToList();

        var realTimeMsgSettingList = session.Query<CustomerMessageRealTimeSendSettingInfo>()
            .Where(e => e.ClientCustomizable == true
                        && e.Status == CustomerMessageSettingStatus.Enabled)
            .ToList();

        var cycleTimeMsgSettingList = session.Query<CustomerMessageCycleTimingSendSettingInfo>()
            .Where(e => e.ClientCustomizable == true
                        && e.Status == CustomerMessageSettingStatus.Enabled)
            .ToList();

        IList<CustomerMessageCustomizeListItem> messageCustomizeList = new List<CustomerMessageCustomizeListItem>();

        foreach (CustomerMessageRealTimeSendSettingInfo realTimeMsgSetting in realTimeMsgSettingList)
        {
            int typeId = realTimeMsgSetting.Type.Id;
            var msgCustomize = msgCustomizeList.Where(e => e.MessageType.Id == typeId).Take(1).SingleOrDefault();

            bool send = true;
            if (msgCustomize != null && msgCustomize.Send == false)
            {
                send = false;
            }

            messageCustomizeList.Add(new CustomerMessageCustomizeListItem() {
                TypeId = typeId,
                Send = send,
                SendHint = GetSendHint(send),
                Title = realTimeMsgSetting.ClientTitle,
                Description = realTimeMsgSetting.ClientDesc,
            });
        }

        foreach (CustomerMessageCycleTimingSendSettingInfo cycleTimeMsgSetting in cycleTimeMsgSettingList)
        {
            int typeId = cycleTimeMsgSetting.Type.Id;
            var msgCustomize = msgCustomizeList.Where(e => e.MessageType.Id == typeId).Take(1).SingleOrDefault();

            bool send = true;
            if (msgCustomize != null && msgCustomize.Send == false)
            {
                send = false;
            }

            messageCustomizeList.Add(new CustomerMessageCustomizeListItem() {
                TypeId = typeId,
                Send = send,
                SendHint = GetSendHint(send),
                Title = cycleTimeMsgSetting.ClientTitle,
                Description = cycleTimeMsgSetting.ClientDesc,
            });
        }

        return messageCustomizeList.OrderBy(e => e.TypeId).ToList(); // 按 type id 排序
    }

    [NonAction]
    private string GetSendHint(bool send)
    {
        if (send == true)
        {
            return "接收消息";
        }

        return "不接收消息";
    }

    [HttpPost]
    [Route("/client/customer/message/customize/save")]
    public IActionResult Save([FromBody] CustomerMessageCustomizeSaveParam param)
    {
        if (ModelState.IsValid == false)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        CustomerMessageType messageType = CustomerMessageType.GetById(param.TypeId);

        var messageCustomize = session.Query<CustomerMessageClientCustomizeInfo>()
            .Where(e => e.CustomerId == loginCustomerId && e.MessageType == messageType)
            .Take(1)
            .SingleOrDefault();

        if (messageCustomize == null)
        {
            messageCustomize = new CustomerMessageClientCustomizeInfo() {
                CustomerId = loginCustomerId,
                MessageType = messageType,
            };
        }

        messageCustomize.Send = param.Send;

        session.SaveOrUpdate(messageCustomize);
        session.Flush();

        IList<CustomerMessageCustomizeListItem> messageCustomizeList = GetMessageCustomizeList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CustomerMessageCustomizeSaveModel() {
                MessageCustomizeList = messageCustomizeList,
            }
        });
    }
}
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Message;

namespace shunda.Admin.Areas.Message.Models.CustomerMessage;

public class CustomerMessageIndexModel
{
    [BindNever]
    public IList<CustomerMessageSendType> SendTypeList { get; set; } = CustomerMessageSendType.GetList();

    [BindNever]
    public IList<CustomerMessageType> MessageTypeList { get; set; } = CustomerMessageType.GetList();
}
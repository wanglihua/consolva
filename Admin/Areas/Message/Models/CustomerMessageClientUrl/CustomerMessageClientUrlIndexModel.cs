using shunda.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Message;

namespace shunda.Admin.Areas.Message.Models.CustomerMessageClientUrl;

public class CustomerMessageClientUrlIndexModel
{
    [BindNever]
    public IList<CustomerMessageType> MessageTypeList { get; set; } = new List<CustomerMessageType>();

    [BindNever]
    public IList<TerminalType> TerminalTypeList { get; set; } = new List<TerminalType>();

}
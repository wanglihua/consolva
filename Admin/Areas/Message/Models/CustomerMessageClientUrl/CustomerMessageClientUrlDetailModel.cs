using System.ComponentModel.DataAnnotations;
using shunda.Domain.Message;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain;

namespace shunda.Admin.Areas.Message.Models.CustomerMessageClientUrl;

public class CustomerMessageClientUrlDetailModel : CustomerMessageClientUrlSaveModel
{
    [BindNever]
    public IList<CustomerMessageType> MessageTypeList { get; set; } = new List<CustomerMessageType>();

    [BindNever]
    public IList<TerminalType> TerminalTypeList { get; set; } = new List<TerminalType>();

}
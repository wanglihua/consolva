using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Message;

namespace shunda.Admin.Areas.Message.Models.CustomerMessage;

public class CustomerMessageListQueryParam : ListQueryParam
{
    public string? CustomerCodeQuery { get; set; }

    [ModelBinder(typeof(CustomerMessageTypeModelBinder))]
    public CustomerMessageType? MessageTypeQuery { get; set; }

    [ModelBinder(typeof(CustomerMessageSendTypeModelBinder))]
    public CustomerMessageSendType? SendTypeQuery { get; set; }

    public string? ContentQuery { get; set; }

    public bool? SendByWechatQuery { get; set; }

    public bool? SendBySmsQuery { get; set; }

    public string? OrderCodeQuery { get; set; }

    [BindNever]
    public int? OrderId { get; set; } = null;
}
using System.ComponentModel.DataAnnotations;
using shunda.Domain.Message;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain;

namespace shunda.Admin.Areas.Message.Models.CustomerMessageClientUrl;

public class CustomerMessageClientUrlSaveModel
{
    /// <summary>
    /// Id
    /// </summary>
    public int CustomerMessageClientUrlId { get; set; }

    /// <summary>
    /// 消息类型，非空
    /// </summary>
    [Required(ErrorMessage = "消息类型不能为空！")]
    [ModelBinder(typeof(CustomerMessageTypeModelBinder))]
    public CustomerMessageType MessageType { get; set; } = CustomerMessageType.None;

    /// <summary>
    /// 终端类型，非空
    /// </summary>
    [Required(ErrorMessage = "终端类型不能为空！")]
    [ModelBinder(typeof(TerminalTypeModelBinder))]
    public TerminalType TerminalType { get; set; } = TerminalType.None;

    /// <summary>
    /// 客户端URL，非空
    /// </summary>
    [Required(ErrorMessage = "客户端URL不能为空！")]
    [MaxLength(200, ErrorMessage = "客户端URL长度不能大于200！")]
    public string ClientUrl { get; set; } = string.Empty;

}
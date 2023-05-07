using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace shunda.Domain.Message;

/// <summary>
/// 客户消息客户端URL
/// 这个现在基本上不用了
/// </summary>
[Obsolete]
public class CustomerMessageClientUrlInfo : Entity
{
    /// <summary>
    /// 消息类型，非空
    /// </summary>
    [Required(ErrorMessage = "消息类型不能为空！")]
    [JsonProperty("messageType")]
    [ModelBinder(typeof(CustomerMessageTypeModelBinder))]
    public virtual CustomerMessageType MessageType { get; set; } = CustomerMessageType.None;

    /// <summary>
    /// 终端类型，非空
    /// </summary>
    [Required(ErrorMessage = "终端类型不能为空！")]
    [JsonProperty("terminalType")]
    [ModelBinder(typeof(TerminalTypeModelBinder))]
    public virtual TerminalType TerminalType { get; set; } = TerminalType.None;

    /// <summary>
    /// 客户端URL，非空
    /// </summary>
    [Required(ErrorMessage = "客户端URL不能为空！")]
    [MaxLength(200, ErrorMessage = "客户端URL长度不能大于200！")]
    [JsonProperty("clientUrl")]
    public virtual string ClientUrl { get; set; } = string.Empty;

}
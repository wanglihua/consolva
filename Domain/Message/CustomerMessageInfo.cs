using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;

namespace shunda.Domain.Message;

/// <summary>
/// 客户消息
/// </summary>
public class CustomerMessageInfo : EntityNoUpdate
{
    /// <summary>
    /// 消息发送类型，非空
    /// </summary>
    [Required(ErrorMessage = "消息发送类型不能为空！")]
    [JsonProperty("sendType")]
    [ModelBinder(typeof(CustomerMessageSendTypeModelBinder))]
    public virtual CustomerMessageSendType SendType { get; set; } = CustomerMessageSendType.None;

    /// <summary>
    /// 消息类型，非空
    /// </summary>
    [Required(ErrorMessage = "消息类型不能为空！")]
    [JsonProperty("type")]
    [ModelBinder(typeof(CustomerMessageTypeModelBinder))]
    public virtual CustomerMessageType Type { get; set; } = CustomerMessageType.None;

    /// <summary>
    /// 客户，非空
    /// </summary>
    [Required(ErrorMessage = "客户不能为空！")]
    [JsonProperty("customer")]
    public virtual CustomerInfo Customer { get; set; } = new ();

    /// <summary>
    /// 物流订单，可空
    /// </summary>
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo? Order { get; set; } = null;

    /// <summary>
    /// 消息内容，非空
    /// </summary>
    [Required(ErrorMessage = "消息内容不能为空！")]
    [MaxLength(400, ErrorMessage = "消息内容长度不能大于400！")]
    [JsonProperty("content")]
    public virtual string Content { get; set; } = string.Empty;

    /// <summary>
    /// 发送状态，非空
    /// </summary>
    [Required(ErrorMessage = "发送状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(CustomerMessageStatusModelBinder))]
    public virtual CustomerMessageStatus Status { get; set; } = CustomerMessageStatus.None;

    /// <summary>
    /// 是否SMS发送，非空
    /// </summary>
    [Required(ErrorMessage = "是否SMS发送不能为空！")]
    [JsonProperty("smsSend")]
    public virtual bool SmsSend { get; set; } = false;

    /// <summary>
    /// 是否微信发送，非空
    /// </summary>
    [Required(ErrorMessage = "是否微信发送不能为空！")]
    [JsonProperty("wechatSend")]
    public virtual bool WechatSend { get; set; } = false;

    /// <summary>
    /// 发送时间，非空
    /// </summary>
    [Required(ErrorMessage = "发送时间不能为空！")]
    [JsonProperty("sendTime")]
    public virtual DateTime SendTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// 发送者，非空
    /// </summary>
    [Required(ErrorMessage = "发送时间不能为空！")]
    [JsonProperty("sender")]
    public virtual string Sender { get; set; } = string.Empty;
    
    #region Cache

    public virtual string CustomerCodeCache { get; set; } = string.Empty;

    #endregion
}
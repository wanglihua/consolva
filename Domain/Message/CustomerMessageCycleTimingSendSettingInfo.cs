using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace shunda.Domain.Message;

/// <summary>
/// 定时任务推送设置
/// </summary>
public class CustomerMessageCycleTimingSendSettingInfo : Entity
{
    /// <summary>
    /// 消息类型，非空
    /// </summary>
    [Required(ErrorMessage = "消息类型不能为空！")]
    [JsonProperty("type")]
    [ModelBinder(typeof(CustomerMessageTypeModelBinder))]
    public virtual CustomerMessageType Type { get; set; } = CustomerMessageType.None;

    /// <summary>
    /// 微信消息模板1，非空
    /// </summary>
    [Required(ErrorMessage = "微信消息模板1不能为空！")]
    [MaxLength(500, ErrorMessage = "微信消息模板1长度不能大于500！")]
    [JsonProperty("wechatMessageTemplate1")]
    public virtual string WechatMessageTemplate1 { get; set; } = string.Empty;

    /// <summary>
    /// 微信消息模板2，非空
    /// </summary>
    [Required(ErrorMessage = "微信消息模板2不能为空！")]
    [MaxLength(500, ErrorMessage = "微信消息模板2长度不能大于500！")]
    [JsonProperty("wechatMessageTemplate2")]
    public virtual string WechatMessageTemplate2 { get; set; } = string.Empty;

    /// <summary>
    /// 微信消息模板3，非空
    /// </summary>
    [Required(ErrorMessage = "微信消息模板3不能为空！")]
    [MaxLength(500, ErrorMessage = "微信消息模板3长度不能大于500！")]
    [JsonProperty("wechatMessageTemplate3")]
    public virtual string WechatMessageTemplate3 { get; set; } = string.Empty;

    /// <summary>
    /// 短信消息模板，非空
    /// </summary>
    [Required(ErrorMessage = "短信消息模板不能为空！")]
    [MaxLength(500, ErrorMessage = "短信消息模板长度不能大于500！")]
    [JsonProperty("smsMessageTemplate")]
    public virtual string SmsMessageTemplate { get; set; } = string.Empty;

    /// <summary>
    /// 启用状态，非空
    /// </summary>
    [Required(ErrorMessage = "启用状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(CustomerMessageSettingStatusModelBinder))]
    public virtual CustomerMessageSettingStatus Status { get; set; } = CustomerMessageSettingStatus.None;

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
    /// 占位符，非空
    /// </summary>
    [Required(ErrorMessage = "占位符不能为空！")]
    [MaxLength(300, ErrorMessage = "占位符长度不能大于300！")]
    [JsonProperty("placeholders")]
    public virtual string Placeholders { get; set; } = string.Empty;

    /// <summary>
    /// 周期类型，非空
    /// </summary>
    [Required(ErrorMessage = "周期类型不能为空！")]
    [JsonProperty("intervalType")]
    [ModelBinder(typeof(CustomerMessageCycleTimingSendIntervalTypeModelBinder))]
    public virtual CustomerMessageCycleTimingSendIntervalType IntervalType { get; set; } = CustomerMessageCycleTimingSendIntervalType.None;

    /// <summary>
    /// 发送周期（周），非空
    /// </summary>
    [Required(ErrorMessage = "发送周期（周）不能为空！")]
    [JsonProperty("intervalWeeks")]
    public virtual int IntervalWeeks { get; set; } = 0;

    /// <summary>
    /// 发送周期（天），非空
    /// </summary>
    [Required(ErrorMessage = "发送周期（天）不能为空！")]
    [JsonProperty("intervalDays")]
    public virtual int IntervalDays { get; set; } = 0;

    /// <summary>
    /// 星期几发送，非空
    /// </summary>
    [Required(ErrorMessage = "星期几发送不能为空！")]
    [JsonProperty("sendWeekday")]
    public virtual int SendWeekday { get; set; } = 0;

    /// <summary>
    /// 发送时间，非空
    /// </summary>
    [Required(ErrorMessage = "发送时间不能为空！")]
    [JsonProperty("sendHour")]
    public virtual int SendHour { get; set; } = 0;

    /// <summary>
    /// 客户端可定制，非空
    /// </summary>
    [Required(ErrorMessage = "客户端可定制不能为空！")]
    [JsonProperty("clientCustomizable")]
    public virtual bool ClientCustomizable { get; set; } = false;

    /// <summary>
    /// 客户端标题，非空
    /// </summary>
    [Required(ErrorMessage = "客户端标题不能为空！")]
    [MaxLength(100, ErrorMessage = "客户端标题长度不能大于100！")]
    [JsonProperty("clientTitle")]
    public virtual string ClientTitle { get; set; } = string.Empty;

    /// <summary>
    /// 客户端描述，非空
    /// </summary>
    [Required(ErrorMessage = "客户端描述不能为空！")]
    [MaxLength(500, ErrorMessage = "客户端描述长度不能大于500！")]
    [JsonProperty("clientDesc")]
    public virtual string ClientDesc { get; set; } = string.Empty;

}
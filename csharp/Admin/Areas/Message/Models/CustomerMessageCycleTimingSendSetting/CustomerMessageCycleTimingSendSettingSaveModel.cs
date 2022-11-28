using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Message;

namespace shunda.Admin.Areas.Message.Models.CustomerMessageCycleTimingSendSetting;

public class CustomerMessageCycleTimingSendSettingSaveModel
{
    private int SettingId { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    [Required(ErrorMessage = "消息类型不能为空！")]
    [JsonProperty("type")]
    [ModelBinder(typeof(CustomerMessageTypeModelBinder))]
    public CustomerMessageType Type { get; set; } = CustomerMessageType.None;

    /// <summary>
    /// 微信消息模板1
    /// </summary>
    [Required(ErrorMessage = "微信消息模板1不能为空！")]
    [MaxLength(500, ErrorMessage = "微信消息模板1长度不能大于500！")]
    [JsonProperty("wechatMessageTemplate1")]
    public string WechatMessageTemplate1 { get; set; } = string.Empty;

    /// <summary>
    /// 微信消息模板2
    /// </summary>
    [MaxLength(500, ErrorMessage = "微信消息模板2长度不能大于500！")]
    [JsonProperty("wechatMessageTemplate2")]
    public string? WechatMessageTemplate2 { get; set; }

    /// <summary>
    /// 微信消息模板3
    /// </summary>
    [MaxLength(500, ErrorMessage = "微信消息模板3长度不能大于500！")]
    [JsonProperty("wechatMessageTemplate3")]
    public string? WechatMessageTemplate3 { get; set; } = string.Empty;

    /// <summary>
    /// 短信消息模板
    /// </summary>
    [Required(ErrorMessage = "短信消息模板不能为空！")]
    [MaxLength(500, ErrorMessage = "短信消息模板长度不能大于500！")]
    [JsonProperty("smsMessageTemplate")]
    public string SmsMessageTemplate { get; set; } = string.Empty;

    /// <summary>
    /// 启用状态
    /// </summary>
    [Required(ErrorMessage = "启用状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(CustomerMessageSettingStatusModelBinder))]
    public CustomerMessageSettingStatus Status { get; set; } = CustomerMessageSettingStatus.None;

    /// <summary>
    /// 是否SMS发送
    /// </summary>
    [Required(ErrorMessage = "是否SMS发送不能为空！")]
    [JsonProperty("smsSend")]
    public bool SmsSend { get; set; } = false;

    /// <summary>
    /// 是否微信发送
    /// </summary>
    [Required(ErrorMessage = "是否微信发送不能为空！")]
    [JsonProperty("wechatSend")]
    public bool WechatSend { get; set; } = false;

    /// <summary>
    /// 周期类型
    /// </summary>
    [Required(ErrorMessage = "周期类型不能为空！")]
    [JsonProperty("intervalType")]
    [ModelBinder(typeof(CustomerMessageCycleTimingSendIntervalTypeModelBinder))]
    public CustomerMessageCycleTimingSendIntervalType IntervalType { get; set; } = CustomerMessageCycleTimingSendIntervalType.None;

    /// <summary>
    /// 发送周期（周）
    /// </summary>
    [Required(ErrorMessage = "发送周期（周）不能为空！")]
    [JsonProperty("intervalWeeks")]
    public int IntervalWeeks { get; set; } = 0;

    /// <summary>
    /// 发送周期（天）
    /// </summary>
    [Required(ErrorMessage = "发送周期（天）不能为空！")]
    [JsonProperty("intervalDays")]
    public int IntervalDays { get; set; } = 0;

    /// <summary>
    /// 星期几发送
    /// </summary>
    [Required(ErrorMessage = "星期几发送不能为空！")]
    [JsonProperty("sendWeekday")]
    public int SendWeekday { get; set; } = 0;

    /// <summary>
    /// 发送时间
    /// </summary>
    [Required(ErrorMessage = "发送时间不能为空！")]
    [JsonProperty("sendHour")]
    public int SendHour { get; set; } = 0;

    /// <summary>
    /// 客户端可定制
    /// </summary>
    [Required(ErrorMessage = "客户端可定制不能为空！")]
    [JsonProperty("clientCustomizable")]
    public bool ClientCustomizable { get; set; } = false;

    /// <summary>
    /// 客户端标题
    /// </summary>
    [Required(ErrorMessage = "客户端标题不能为空！")]
    [MaxLength(100, ErrorMessage = "客户端标题长度不能大于100！")]
    [JsonProperty("clientTitle")]
    public string ClientTitle { get; set; } = string.Empty;

    /// <summary>
    /// 客户端描述
    /// </summary>
    [Required(ErrorMessage = "客户端描述不能为空！")]
    [MaxLength(500, ErrorMessage = "客户端描述长度不能大于500！")]
    [JsonProperty("clientDesc")]
    public string ClientDesc { get; set; } = string.Empty;
}
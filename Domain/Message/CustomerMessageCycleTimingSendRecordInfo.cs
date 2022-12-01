using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace shunda.Domain.Message;

/// <summary>
/// 定时任务推送记录
/// </summary>
public class CustomerMessageCycleTimingSendRecordInfo : EntityNoUpdate
{
    /// <summary>
    /// 消息类型，非空
    /// </summary>
    [Required(ErrorMessage = "消息类型不能为空！")]
    [JsonProperty("type")]
    [ModelBinder(typeof(CustomerMessageTypeModelBinder))]
    public virtual CustomerMessageType Type { get; set; } = CustomerMessageType.None;

    /// <summary>
    /// 启用状态，非空
    /// </summary>
    [Required(ErrorMessage = "启用状态不能为空！")]
    [JsonProperty("settingStatus")]
    [ModelBinder(typeof(CustomerMessageSettingStatusModelBinder))]
    public virtual CustomerMessageSettingStatus SettingStatus { get; set; } = CustomerMessageSettingStatus.None;

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
}
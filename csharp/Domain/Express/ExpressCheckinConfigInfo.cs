using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单入库配置
/// </summary>
public class ExpressCheckinConfigInfo : Entity
{
    /// <summary>
    /// 每仓位快递数，非空
    /// </summary>
    [Required(ErrorMessage = "每仓位快递数不能为空！")]
    [JsonProperty("expressCountPerStorageLoc")]
    public virtual int ExpressCountPerStorageLoc { get; set; } = 0;

    /// <summary>
    /// 签收照片最小数量，非空
    /// </summary>
    [Required(ErrorMessage = "签收照片最小数量不能为空！")]
    [JsonProperty("signinPhotoMinQtyPerExpress")]
    public virtual int SigninPhotoMinQtyPerExpress { get; set; } = 0;

    /// <summary>
    /// 入库照片最小数量，非空
    /// </summary>
    [Required(ErrorMessage = "入库照片最小数量不能为空！")]
    [JsonProperty("checkinPhotoMinQtyPerExpress")]
    public virtual int CheckinPhotoMinQtyPerExpress { get; set; } = 0;

    /// <summary>
    /// 是否自动录制视频，非空
    /// </summary>
    [Required(ErrorMessage = "是否自动录制视频不能为空！")]
    [JsonProperty("autoRecordCheckinVideo")]
    public virtual bool AutoRecordCheckinVideo { get; set; } = false;

    /// <summary>
    /// 客户端是否显示视频，非空
    /// </summary>
    [Required(ErrorMessage = "客户端是否显示视频不能为空！")]
    [JsonProperty("clientShowVideo")]
    public virtual bool ClientShowVideo { get; set; } = false;

    /// <summary>
    /// 客户端是否显示入库照片，非空
    /// </summary>
    [Required(ErrorMessage = "客户端是否显示入库照片不能为空！")]
    [JsonProperty("checkinPhotoClientShow")]
    public virtual bool CheckinPhotoClientShow { get; set; } = false;

    /// <summary>
    /// 标签10*5是否启用，非空
    /// </summary>
    [Required(ErrorMessage = "标签10*5是否启用不能为空！")]
    [JsonProperty("labelW10H05Status")]
    [ModelBinder(typeof(ExpressCheckinLabelConfigStatusModelBinder))]
    public virtual ExpressCheckinLabelConfigStatus LabelW10H05Status { get; set; } = ExpressCheckinLabelConfigStatus.None;

    /// <summary>
    /// 标签10*5打印数量，非空
    /// </summary>
    [Required(ErrorMessage = "标签10*5打印数量不能为空！")]
    [JsonProperty("labelW10H05PrintCount")]
    public virtual int LabelW10H05PrintCount { get; set; } = 0;

    /// <summary>
    /// 标签5*5是否启用，非空
    /// </summary>
    [Required(ErrorMessage = "标签5*5是否启用不能为空！")]
    [JsonProperty("labelW05H05Status")]
    [ModelBinder(typeof(ExpressCheckinLabelConfigStatusModelBinder))]
    public virtual ExpressCheckinLabelConfigStatus LabelW05H05Status { get; set; } = ExpressCheckinLabelConfigStatus.None;

    /// <summary>
    /// 标签5*5打印数量，非空
    /// </summary>
    [Required(ErrorMessage = "标签5*5打印数量不能为空！")]
    [JsonProperty("labelW05H05PrintCount")]
    public virtual int LabelW05H05PrintCount { get; set; } = 0;

}
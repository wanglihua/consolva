using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using shunda.Core;

namespace shunda.Domain.Express;

/// <summary>
/// 入库跟踪单号
/// 使用了存储过程，这个类就基本上没用起来
/// </summary>
public class CheckinTrackingNoInfo : Entity
{
    /// <summary>
    /// 年份，非空
    /// </summary>
    [Required(ErrorMessage = "年份不能为空！")]
    [JsonProperty("year")]
    public virtual int Year { get; set; } = 0;

    /// <summary>
    /// 月份，非空
    /// </summary>
    [Required(ErrorMessage = "月份不能为空！")]
    [JsonProperty("month")]
    public virtual int Month { get; set; } = 0;

    /// <summary>
    /// 序列号，非空
    /// </summary>
    [Required(ErrorMessage = "序列号不能为空！")]
    [JsonProperty("seq")]
    public virtual int Seq { get; set; } = 0;
}
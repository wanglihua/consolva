using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using shunda.Core;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 订单备注原因
/// </summary>
public class LogisticsOrderRemarkReasonInfo : Entity
{
    /// <summary>
    /// 原因，非空
    /// </summary>
    [Required(ErrorMessage = "原因不能为空！")]
    [MaxLength(100, ErrorMessage = "原因长度不能大于100！")]
    [JsonProperty("reason")]
    public virtual string Reason { get; set; } = string.Empty;

    /// <summary>
    /// 描述，非空
    /// </summary>
    [Required(ErrorMessage = "描述不能为空！")]
    [MaxLength(300, ErrorMessage = "描述长度不能大于300！")]
    [JsonProperty("description")]
    public virtual string Description { get; set; } = string.Empty;

}
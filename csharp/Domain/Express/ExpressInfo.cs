using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace shunda.Domain.Express;

/// <summary>
/// 商品快递
/// </summary>
public class ExpressInfo : Entity
{
    /// <summary>
    /// 快递单号，非空
    /// </summary>
    [Required(ErrorMessage = "快递单号不能为空！")]
    [MaxLength(30, ErrorMessage = "快递单号长度不能大于30！")]
    [JsonProperty("trackingNo")]
    public virtual string TrackingNo { get; set; } = string.Empty;

}
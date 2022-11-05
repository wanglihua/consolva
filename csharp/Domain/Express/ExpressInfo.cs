using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using shunda.Core;
using shunda.Domain.Order;

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

    /// <summary>
    /// 物流订单，可空
    /// </summary>
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo? Order { get; set; } = null;
}
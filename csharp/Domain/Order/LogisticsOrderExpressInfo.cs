using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Domain.Order;

/// <summary>
/// 订单快递单号
/// 作废订单才需要这个记录
/// </summary>
public class LogisticsOrderExpressInfo : Entity
{
    /// <summary>
    /// 订单，非空
    /// </summary>
    [Required(ErrorMessage = "订单不能为空！")]
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo Order { get; set; } = new LogisticsOrderInfo();

    /// <summary>
    /// 快递单号，非空
    /// </summary>
    [Required(ErrorMessage = "快递单号不能为空！")]
    [JsonProperty("express")]
    public virtual ExpressInfo Express { get; set; } = new ExpressInfo();
}
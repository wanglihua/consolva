using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 订单打包快递单号
/// </summary>
public class LogisticsOrderPackExpressInfo : Entity
{
    /// <summary>
    /// 订单，非空
    /// </summary>
    [Required(ErrorMessage = "订单不能为空！")]
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo Order { get; set; } = new LogisticsOrderInfo();

    /// <summary>
    /// 订单打包，非空
    /// </summary>
    [Required(ErrorMessage = "订单打包不能为空！")]
    [JsonProperty("orderPack")]
    public virtual LogisticsOrderPackInfo OrderPack { get; set; } = new LogisticsOrderPackInfo();

    /// <summary>
    /// 快递单，非空
    /// </summary>
    [Required(ErrorMessage = "快递单不能为空！")]
    [JsonProperty("express")]
    public virtual ExpressInfo Express { get; set; } = new ExpressInfo();

}
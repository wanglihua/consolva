using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Pass;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 订单打包箱号
/// </summary>
public class LogisticsOrderPackBoxInfo : Entity
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
    /// 订单打包箱序，非空
    /// </summary>
    [Required(ErrorMessage = "订单打包箱序不能为空！")]
    [JsonProperty("boxNum")]
    public virtual int BoxNum { get; set; } = 0;

    /// <summary>
    /// 发货时间，可空
    /// </summary>
    [JsonProperty("shipTime")]
    public virtual DateTime? ShipTime { get; set; } = null;

    /// <summary>
    /// 发货人，可空
    /// </summary>
    [JsonProperty("shipUser")]
    public virtual UserInfo? ShipUser { get; set; } = null;

    /// <summary>
    /// 交付时间，可空
    /// </summary>
    [JsonProperty("deliverTime")]
    public virtual DateTime? DeliverTime { get; set; } = null;

    /// <summary>
    /// 状态，非空
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(LogisticsOrderPackBoxStatusModelBinder))]
    public virtual LogisticsOrderPackBoxStatus Status { get; set; } = LogisticsOrderPackBoxStatus.None;
}
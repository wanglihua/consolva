using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Logistics;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 物流订单头程
/// </summary>
public class LogisticsOrderFirstModeInfo : Entity
{
    /// <summary>
    /// 物流订单，非空
    /// </summary>
    [Required(ErrorMessage = "物流订单不能为空！")]
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo Order { get; set; } = new LogisticsOrderInfo();

    /// <summary>
    /// 订单编号，非空
    /// </summary>
    [Required(ErrorMessage = "订单编号不能为空！")]
    [MaxLength(100, ErrorMessage = "订单编号长度不能大于100！")]
    [JsonProperty("orderCode")]
    public virtual string OrderCode { get; set; } = string.Empty;

    /// <summary>
    /// 物品类型，非空
    /// </summary>
    [Required(ErrorMessage = "物品类型不能为空！")]
    [JsonProperty("goodsType")]
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public virtual LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 物流方式类型，非空
    /// </summary>
    [Required(ErrorMessage = "物流方式类型不能为空！")]
    [JsonProperty("firstType")]
    [ModelBinder(typeof(LogisticsModeTypeModelBinder))]
    public virtual LogisticsModeType FirstType { get; set; } = LogisticsModeType.None;

    /// <summary>
    /// 物流方式，非空
    /// </summary>
    [Required(ErrorMessage = "物流方式不能为空！")]
    [JsonProperty("firstMode")]
    public virtual LogisticsModeInfo FirstMode { get; set; } = new LogisticsModeInfo();

}
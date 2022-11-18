using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 国际单号
/// </summary>
public class LogisticsOrderAirExpressInfo : Entity
{
    /// <summary>
    /// 订单，可空
    /// </summary>
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo? Order { get; set; } = null;

    /// <summary>
    /// 订单编号，非空
    /// </summary>
    public virtual string OrderCode { get; set; } = string.Empty;

    /// <summary>
    /// 物流方式，可空
    /// </summary>
    [JsonProperty("logisticsMode")]
    public virtual LogisticsModeInfo? LogisticsMode { get; set; } = null;

    /// <summary>
    /// 国际快递单号，非空
    /// </summary>
    [Required(ErrorMessage = "国际快递单号不能为空！")]
    [MaxLength(30, ErrorMessage = "国际快递单号长度不能大于30！")]
    [JsonProperty("trackingNo")]
    public virtual string TrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 承运人计费重量Kg，可空
    /// </summary>
    [JsonProperty("shippingWeight")]
    public virtual decimal? ShippingWeight { get; set; } = null;

    /// <summary>
    /// 预估运费支出RMB，可空
    /// </summary>
    [JsonProperty("estimateFee")]
    public virtual decimal? EstimateFee { get; set; } = null;

    /// <summary>
    /// 运费支出RMB，可空
    /// </summary>
    [JsonProperty("shippingFee")]
    public virtual decimal? ShippingFee { get; set; } = null;

    /// <summary>
    /// 发货渠道，可空
    /// </summary>
    [MaxLength(50, ErrorMessage = "发货渠道长度不能大于50！")]
    [JsonProperty("channel")]
    public virtual string? Channel { get; set; } = null;

    /// <summary>
    /// 空运备注，可空
    /// </summary>
    [MaxLength(300, ErrorMessage = "空运备注长度不能大于300！")]
    [JsonProperty("memo")]
    public virtual string? Memo { get; set; } = null;

    /// <summary>
    /// 导入人，可空
    /// </summary>
    [JsonProperty("importUser")]
    public virtual UserInfo? ImportUser { get; set; } = null;

    /// <summary>
    /// 导入时间，可空
    /// </summary>
    [JsonProperty("importTime")]
    public virtual DateTime? ImportTime { get; set; } = null;

    /// <summary>
    /// 签收时间，可空
    /// </summary>
    [JsonProperty("deliveredTime")]
    public virtual DateTime? DeliveredTime { get; set; } = null;

    /// <summary>
    /// 状态，非空
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(LogisticsOrderAirExpressStatusModelBinder))]
    public virtual LogisticsOrderAirExpressStatus Status { get; set; } = LogisticsOrderAirExpressStatus.None;

}
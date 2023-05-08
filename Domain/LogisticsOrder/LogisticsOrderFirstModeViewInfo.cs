using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Core;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsTrack;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 物流订单头程
/// 一个物流方式方式，如果一个订单打包时取有多个物品类型的话，会有多条记录
/// </summary>
public class LogisticsOrderFirstModeViewInfo : Entity {
    /// <summary>
    /// 物流订单，非空
    /// </summary>
    [Required(ErrorMessage = "物流订单不能为空！")]
    [JsonProperty("orderId")]
    public virtual int OrderId { get; set; }

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
    public virtual LogisticsModeInfo FirstMode { get; set; } = new();

    /// <summary>
    /// 渠道类型，非空
    /// </summary>
    [ModelBinder(typeof(LogisticsModeChannelModelBinder))]
    public virtual LogisticsModeChannel Channel { get; set; } = LogisticsModeChannel.None;

    /// <summary>
    /// 体积重分母，非空
    /// </summary>
    public virtual int VolumeWeightDenominator { get; set; }

    /// <summary>
    /// 快递单号，这个是国际快递单号
    /// </summary>
    public virtual string TrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 17TRACK快递单号
    /// </summary>
    public virtual SeventeenTrackNumberInfo? SeventeenTrackNumber { get; set; } = null;

    /// <summary>
    /// 跟踪状态
    /// </summary>
    public virtual SeventeenTrackNumberTrackStatus TrackStatus { get; set; } = SeventeenTrackNumberTrackStatus.None;

    /// <summary>
    /// 跟踪子状态
    /// </summary>
    public virtual SeventeenTrackNumberTrackSubStatus TrackSubStatus { get; set; } = SeventeenTrackNumberTrackSubStatus.None;

    #region Cache

    /// <summary>
    /// 订单头程对应的订单打包列表
    /// </summary>
    public virtual IList<LogisticsOrderPackInfo> OrderPackListCache { get; set; } = new List<LogisticsOrderPackInfo>();

    #endregion
}
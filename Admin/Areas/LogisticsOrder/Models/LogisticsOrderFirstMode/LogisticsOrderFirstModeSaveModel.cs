using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderFirstMode;

public class LogisticsOrderFirstModeSaveModel {
    /// <summary>
    /// Id
    /// </summary>
    public int OrderFirstModeId { get; set; }

    /// <summary>
    /// 物流订单，非空
    /// </summary>
    public int OrderId { get; set; } = 0;

    /// <summary>
    /// 订单编号，非空
    /// </summary>
    public string OrderCode { get; set; } = string.Empty;

    /// <summary>
    /// 物品类型，非空
    /// </summary>
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 物流方式类型，非空
    /// </summary>
    [ModelBinder(typeof(LogisticsModeTypeModelBinder))]
    public LogisticsModeType FirstType { get; set; } = LogisticsModeType.None;

    /// <summary>
    /// 物流方式，非空
    /// </summary>
    public int FirstModeId { get; set; } = 0;

    /// <summary>
    /// 渠道类型，非空
    /// </summary>
    [ModelBinder(typeof(LogisticsModeChannelModelBinder))]
    public LogisticsModeChannel Channel { get; set; } = LogisticsModeChannel.None;

    /// <summary>
    /// 体积重分母，非空
    /// </summary>
    public int VolumeWeightDenominator { get; set; } = 0;

    /// <summary>
    /// 快递单号，非空
    /// </summary>
    public string TrackingNo { get; set; } = string.Empty;

}
using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流方式通用费率
/// </summary>
public class LogisticsModeFeeRateInfo : Entity
{
    /// <summary>
    /// 物流方式，非空
    /// </summary>
    [Required(ErrorMessage = "物流方式不能为空！")]
    [JsonProperty("mode")]
    public virtual LogisticsModeInfo Mode { get; set; } = new LogisticsModeInfo();

    /// <summary>
    /// 物品类型，非空
    /// </summary>
    [Required(ErrorMessage = "物品类型不能为空！")]
    [JsonProperty("goodsType")]
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public virtual LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 通用费率，非空
    /// </summary>
    [Required(ErrorMessage = "通用费率不能为空！")]
    [JsonProperty("feeRate")]
    public virtual LogisticsFeeRateInfo FeeRate { get; set; } = new LogisticsFeeRateInfo();

}
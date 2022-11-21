using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 物流订单打包
/// </summary>
public class LogisticsOrderPackInfo : Entity
{
    /// <summary>
    /// 订单，非空
    /// </summary>
    [Required(ErrorMessage = "订单不能为空！")]
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo Order { get; set; } = new LogisticsOrderInfo();

    /// <summary>
    /// 物品类型，非空
    /// </summary>
    [Required(ErrorMessage = "物品类型不能为空！")]
    [JsonProperty("goodsType")]
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public virtual LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 物流订单头程，非空
    /// </summary>
    [Required(ErrorMessage = "物流订单头程不能为空！")]
    [JsonProperty("orderFirstMode")]
    public virtual LogisticsOrderFirstModeInfo OrderFirstMode { get; set; } = new LogisticsOrderFirstModeInfo();

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

    /// <summary>
    /// 长/cm，非空
    /// </summary>
    [Required(ErrorMessage = "长/cm不能为空！")]
    [JsonProperty("length")]
    public virtual decimal Length { get; set; } = 0m;

    /// <summary>
    /// 宽/cm，非空
    /// </summary>
    [Required(ErrorMessage = "宽/cm不能为空！")]
    [JsonProperty("width")]
    public virtual decimal Width { get; set; } = 0m;

    /// <summary>
    /// 高/cm，非空
    /// </summary>
    [Required(ErrorMessage = "高/cm不能为空！")]
    [JsonProperty("height")]
    public virtual decimal Height { get; set; } = 0m;

    /// <summary>
    /// 重量/kg，非空
    /// </summary>
    [Required(ErrorMessage = "重量/kg不能为空！")]
    [JsonProperty("weight")]
    public virtual decimal Weight { get; set; } = 0m;

    /// <summary>
    /// 装箱数目，非空
    /// </summary>
    [Required(ErrorMessage = "装箱数目不能为空！")]
    [JsonProperty("packageCount")]
    public virtual int PackageCount { get; set; } = 0;

    /// <summary>
    /// 打包人员，非空
    /// </summary>
    [Required(ErrorMessage = "打包人员不能为空！")]
    [JsonProperty("user")]
    public virtual UserInfo PackUser { get; set; } = new UserInfo();

    /// <summary>
    /// 打包时间，非空
    /// </summary>
    [Required(ErrorMessage = "打包时间不能为空！")]
    [JsonProperty("packTime")]
    public virtual DateTime PackTime { get; set; } = DateTime.MinValue;

}
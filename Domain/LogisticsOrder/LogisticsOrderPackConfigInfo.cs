using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 物流订单打包配置
/// </summary>
public class LogisticsOrderPackConfigInfo : Entity {
    /// <summary>
    /// 订单排序方式，非空
    /// </summary>
    [Required(ErrorMessage = "订单排序方式不能为空！")]
    [JsonProperty("sortType")]
    [ModelBinder(typeof(LogisticsOrderPackSortTypeModelBinder))]
    public virtual LogisticsOrderPackSortType SortType { get; set; } = LogisticsOrderPackSortType.None;

    /// <summary>
    /// 订单显示方式，非空
    /// </summary>
    [Required(ErrorMessage = "订单显示方式不能为空！")]
    [JsonProperty("displayType")]
    [ModelBinder(typeof(LogisticsOrderPackDisplayTypeModelBinder))]
    public virtual LogisticsOrderPackDisplayType DisplayType { get; set; } = LogisticsOrderPackDisplayType.None;

    /// <summary>
    /// 标签10*15是否启用，非空
    /// </summary>
    [Required(ErrorMessage = "标签10*15是否启用不能为空！")]
    [JsonProperty("label1015Enabled")]
    public virtual bool Label1015Enabled { get; set; } = false;

    /// <summary>
    /// 标签10*15打印数量，非空
    /// </summary>
    [Required(ErrorMessage = "标签10*15打印数量不能为空！")]
    [JsonProperty("label1015PrintCount")]
    public virtual int Label1015PrintCount { get; set; } = 0;

    /// <summary>
    /// 标签10*5是否启用，非空
    /// </summary>
    [Required(ErrorMessage = "标签10*5是否启用不能为空！")]
    [JsonProperty("label1005Enabled")]
    public virtual bool Label1005Enabled { get; set; } = false;

    /// <summary>
    /// 标签10*5打印数量，非空
    /// </summary>
    [Required(ErrorMessage = "标签10*5打印数量不能为空！")]
    [JsonProperty("label1005PrintCount")]
    public virtual int Label1005PrintCount { get; set; } = 0;

    /// <summary>
    /// 标签5*5是否启用，非空
    /// </summary>
    [Required(ErrorMessage = "标签5*5是否启用不能为空！")]
    [JsonProperty("label0505Enabled")]
    public virtual bool Label0505Enabled { get; set; } = false;

    /// <summary>
    /// 标签5*5打印数量，非空
    /// </summary>
    [Required(ErrorMessage = "标签5*5打印数量不能为空！")]
    [JsonProperty("label0505PrintCount")]
    public virtual int Label0505PrintCount { get; set; } = 0;

    /// <summary>
    /// 是否允许快递单号手工输入，非空
    /// </summary>
    [Required(ErrorMessage = "是否允许快递单号手工输入不能为空！")]
    [JsonProperty("trackingNoManualInputEnabled")]
    public virtual bool TrackingNoManualInputEnabled { get; set; } = false;

}
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using shunda.Core;
using shunda.Domain.Logistics;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单物品明细
/// </summary>
public class ExpressGoodsInfo : Entity
{
    /// <summary>
    /// 快递单，非空
    /// </summary>
    [Required(ErrorMessage = "快递单不能为空！")]
    [JsonProperty("express")]
    public virtual ExpressInfo Express { get; set; } = new ExpressInfo();

    /// <summary>
    /// 物品扩展名，非空
    /// </summary>
    [Required(ErrorMessage = "物品扩展名不能为空！")]
    [JsonProperty("goods")]
    public virtual LogisticsGoodsInfo Goods { get; set; } = new LogisticsGoodsInfo();

    /// <summary>
    /// 价格，非空
    /// </summary>
    [Required(ErrorMessage = "价格不能为空！")]
    [JsonProperty("price")]
    public virtual decimal Price { get; set; } = 0m;

    /// <summary>
    /// 数量，非空
    /// </summary>
    [Required(ErrorMessage = "数量不能为空！")]
    [JsonProperty("quantity")]
    public virtual int Quantity { get; set; } = 0;

    /// <summary>
    /// 数量单位，非空
    /// </summary>
    [Required(ErrorMessage = "数量单位不能为空！")]
    [MaxLength(8, ErrorMessage = "数量单位长度不能大于8！")]
    [JsonProperty("unit")]
    public virtual string Unit { get; set; } = string.Empty;
}
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using shunda.Core;

namespace shunda.Domain.Logistics;

/// <summary>
/// 中澳快递具体费率
/// </summary>
public class LogisticsFeeRateExpressInfo : Entity {
    /// <summary>
    /// 物流方式id
    /// </summary>
    [Required(ErrorMessage = "物流方式不能为空！")]
    public virtual LogisticsModeInfo LogisticsMode { get; set; } = new LogisticsModeInfo();

    /// <summary>
    /// 起步重量
    /// </summary>
    [Required(ErrorMessage = "起步重量不能为空！")]
    public virtual decimal? StartWeight { get; set; }

    /// <summary>
    /// 起步价格
    /// </summary>
    [Required(ErrorMessage = "起步价格不能为空！")]
    public virtual decimal? StartPrice { get; set; }

    /// <summary>
    /// 阶梯重量
    /// </summary>
    [Required(ErrorMessage = "阶梯重量不能为空！")]
    public virtual decimal? StepWeight { get; set; }

    /// <summary>
    /// 阶梯价格
    /// </summary>
    [Required(ErrorMessage = "阶梯价格不能为空！")]
    public virtual decimal? StepPrice { get; set; }

    /// <summary>
    /// 首重免费最小总重
    /// </summary>
    public virtual decimal? StartFreeMinWeight { get; set; }

    /// <summary>
    /// 重量满减起始重量
    /// </summary>
    public virtual decimal? WeightBreakStart { get; set; }

    /// <summary>
    /// 重量满减优惠重量
    /// </summary>
    public virtual decimal? WeightBreakDiscount { get; set; }
}
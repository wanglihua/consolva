using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsFeeRateExpress;

public class LogisticsFeeRateExpressSaveModel {
    /// <summary>
    /// 物流方式id
    /// </summary>
    [Required(ErrorMessage = "物流方式不能为空！")]
    public int LogisticsModeId { get; set; }

    /// <summary>
    /// 派送费用
    /// </summary>
    [Required(ErrorMessage = "派送费用不能为空！")]
    public decimal? DeliverPrice { get; set; }

    /// <summary>
    /// 起步重量
    /// </summary>
    [Required(ErrorMessage = "起步重量不能为空！")]
    public decimal? StartWeight { get; set; }

    /// <summary>
    /// 起步价格
    /// </summary>
    [Required(ErrorMessage = "起步价格不能为空！")]
    public decimal? StartPrice { get; set; }

    /// <summary>
    /// 阶梯重量
    /// </summary>
    [Required(ErrorMessage = "阶梯重量不能为空！")]
    public decimal? StepWeight { get; set; }

    /// <summary>
    /// 阶梯价格
    /// </summary>
    [Required(ErrorMessage = "阶梯价格不能为空！")]
    public decimal? StepPrice { get; set; }

    /// <summary>
    /// 首重免费最小总重
    /// </summary>
    public decimal? StartFreeMinWeight { get; set; }

    /// <summary>
    /// 重量满减起始重量
    /// </summary>
    public decimal? WeightBreakStart { get; set; }

    /// <summary>
    /// 重量满减优惠重量
    /// </summary>
    public decimal? WeightBreakDiscount { get; set; }
}
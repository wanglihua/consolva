using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.Fee.Models.CouponRuleDiscount;

public class CouponRuleDiscountDetailModel
{
    /// <summary>
    /// Id
    /// </summary>
    public int CouponRuleDiscountId { get; set; }

    /// <summary>
    /// 批次，非空
    /// </summary>
    [Required(ErrorMessage = "批次不能为空！")]
    public int BatchId { get; set; }

    /// <summary>
    /// 折率，非空
    /// </summary>
    [Required(ErrorMessage = "折率不能为空！")]
    public decimal Rate { get; set; }

    /// <summary>
    /// 是否规则还能修改
    /// </summary>
    public bool CanModify { get; set; }
}
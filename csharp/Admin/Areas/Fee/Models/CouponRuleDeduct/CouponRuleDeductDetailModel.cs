using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.Fee.Models.CouponRuleDeduct;

public class CouponRuleDeductDetailModel
{
    /// <summary>
    /// Id
    /// </summary>
    public int CouponRuleDeductId { get; set; }

    /// <summary>
    /// 批次，非空
    /// </summary>
    [Required(ErrorMessage = "批次不能为空！")]
    public int BatchId { get; set; }

    /// <summary>
    /// 抵扣金额，非空
    /// </summary>
    [Required(ErrorMessage = "抵扣金额不能为空！")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 是否规则还能修改
    /// </summary>
    public bool CanModify { get; set; }
}
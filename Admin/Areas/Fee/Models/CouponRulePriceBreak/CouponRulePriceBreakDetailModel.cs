using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.Fee.Models.CouponRulePriceBreak;

public class CouponRulePriceBreakDetailModel
{
    /// <summary>
    /// Id
    /// </summary>
    public int CouponRulePriceBreakId { get; set; }

    /// <summary>
    /// 批次，非空
    /// </summary>
    [Required(ErrorMessage = "批次不能为空！")]
    public int BatchId { get; set; }

    /// <summary>
    /// 消费金额，非空
    /// </summary>
    [Required(ErrorMessage = "消费金额不能为空！")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 折减金额，非空
    /// </summary>
    [Required(ErrorMessage = "折减金额不能为空！")]
    public decimal Discount { get; set; }

    /// <summary>
    /// 是否规则还能修改
    /// </summary>
    public bool CanModify { get; set; }
}
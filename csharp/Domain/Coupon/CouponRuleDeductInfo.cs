using shunda.Core;

namespace shunda.Domain.Coupon;

/// <summary>
/// 优惠券抵扣规则
/// </summary>
public class CouponRuleDeductInfo : Entity
{
    /// <summary>
    /// 批次，非空
    /// </summary>
    public virtual CouponBatchInfo Batch { get; set; } = new();

    /// <summary>
    /// 抵扣金额，非空
    /// </summary>
    public virtual decimal Amount { get; set; }
}
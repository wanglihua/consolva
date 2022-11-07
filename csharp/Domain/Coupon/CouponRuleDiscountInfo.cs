using shunda.Core;

namespace shunda.Domain.Coupon;

/// <summary>
/// 优惠券折扣规则
/// </summary>
public class CouponRuleDiscountInfo : Entity
{
    /// <summary>
    /// 批次，非空
    /// </summary>
    public virtual CouponBatchInfo Batch { get; set; } = new();

    /// <summary>
    /// 折率，非空
    /// </summary>
    public virtual decimal Rate { get; set; }
}
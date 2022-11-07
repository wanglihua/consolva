using shunda.Core;

namespace shunda.Domain.Coupon;

/// <summary>
/// 优惠券满减规则
/// </summary>
public class CouponRulePriceBreakInfo : Entity
{
    /// <summary>
    /// 批次，非空
    /// </summary>
    public virtual CouponBatchInfo Batch { get; set; } = new();

    /// <summary>
    /// 消费金额，非空
    /// </summary>
    public virtual decimal Amount { get; set; }

    /// <summary>
    /// 折减金额，非空
    /// </summary>
    public virtual decimal Discount { get; set; }
}
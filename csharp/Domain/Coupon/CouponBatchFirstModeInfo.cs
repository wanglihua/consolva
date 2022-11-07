using shunda.Core;
using shunda.Domain.Logistics;

namespace shunda.Domain.Coupon;

/// <summary>
/// 优惠券批次头程物流
/// </summary>
public class CouponBatchFirstModeInfo : Entity
{
    /// <summary>
    /// 优惠券批次，非空
    /// </summary>
    public virtual CouponBatchInfo Batch { get; set; } = new();

    /// <summary>
    /// 头程物流，非空
    /// </summary>
    public virtual LogisticsModeInfo FirstMode { get; set; } = new();
}
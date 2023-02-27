using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Coupon;

/// <summary>
/// 优惠券
/// </summary>
public class CouponLogInfo : Entity {
    /// <summary>
    /// 优惠券，非空
    /// </summary>
    public virtual CouponInfo Coupon { get; set; } = new CouponInfo();

    /// <summary>
    /// 类型，非空
    /// </summary>
    public virtual CouponLogType Type { get; set; } = CouponLogType.None;

    /// <summary>
    /// 描述，非空
    /// </summary>
    public virtual string Description { get; set; } = string.Empty;

    public override void Validate() {
        ValidationUtil.ValidateEntity<CouponLogInfoValidator, CouponLogInfo>(this);
    }
}
using shunda.Core;

namespace shunda.Domain.Coupon;

/// <summary>
/// 优惠券批次
/// </summary>
public  class CouponBatchInfo : Entity
{
    /// <summary>
    /// 名称，非空
    /// </summary>
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 获取方式
    /// </summary>
    public virtual CouponGetMethod GetMethod { get; set; } = CouponGetMethod.Pick;

    /// <summary>
    /// 类型，非空
    /// </summary>
    public virtual CouponType Type { get; set; } = CouponType.None;

    /// <summary>
    /// 有效期，非空
    /// </summary>
    public virtual DateTime Expire { get; set; }

    /// <summary>
    /// 有效期月份数，非空
    /// </summary>
    public virtual int ExpireMonth { get; set; }

    /// <summary>
    /// 总数量，非空
    /// </summary>
    public virtual int TotalQuantity { get; set; }

    /// <summary>
    /// 已领用数量，非空
    /// </summary>
    public virtual int PickQuantity { get; set; }

    /// <summary>
    /// 已使用数量，非空
    /// </summary>
    public virtual int UseQuantity { get; set; }

    /// <summary>
    /// 规则，可空
    /// </summary>
    public virtual string Rule { get; set; } = string.Empty;

    /// <summary>
    /// 规则描述，可空
    /// </summary>
    public virtual string RuleDesc { get; set; } = string.Empty;

    /// <summary>
    /// 每客户可领取数量，非空
    /// </summary>
    public virtual int MaxQtyPerCustomer { get; set; }

    /// <summary>
    /// 状态，非空
    /// </summary>
    public virtual CouponBatchStatus Status { get; set; } = CouponBatchStatus.None;
}
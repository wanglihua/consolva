using shunda.Core;
using shunda.Domain.Customer;
using shunda.Domain.Order;

namespace shunda.Domain.Coupon;

/// <summary>
/// 优惠券
/// </summary>
public class CouponInfo : Entity
{
    /// <summary>
    /// 批次，非空
    /// </summary>
    public virtual CouponBatchInfo Batch { get; set; } = new();

    /// <summary>
    /// 编号，非空
    /// </summary>
    public virtual string Code { get; set; } = string.Empty;

    /// <summary>
    /// 客户，非空
    /// </summary>
    public virtual CustomerInfo Customer { get; set; } = new();

    /// <summary>
    /// 订单，可空
    /// </summary>
    public virtual LogisticsOrderInfo? Order { get; set; }

    /// <summary>
    /// 订单编号，非空
    /// 设置 Order的时候记得设置 orderCode
    /// </summary>
    public virtual string OrderCode { get; set; } = string.Empty;

    /// <summary>
    /// 实际优惠金额，非空
    /// </summary>
    public virtual decimal ActualDiscountAmount { get; set; }

    /// <summary>
    /// 领取时间，非空
    /// </summary>
    public virtual DateTime PickTime { get; set; }

    /// <summary>
    /// 使用时间，可空
    /// </summary>
    public virtual DateTime? UseTime { get; set; }

    /// <summary>
    /// 获取方式
    /// </summary>
    public virtual CouponGetMethod GetMethod { get; set; } = CouponGetMethod.Pick;

    /// <summary>
    /// 状态，非空
    /// </summary>
    public virtual CouponStatus Status { get; set; } = CouponStatus.None;

    #region Cache

    public virtual string CustomerCodeCache { get; set; } = string.Empty;

    #endregion
}
using shunda.Core;
using shunda.Domain.Logistics;

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
    /// 类型，非空
    /// </summary>
    public virtual CouponType Type { get; set; } = CouponType.None;

    /// <summary>
    /// 头程类型，这个字段后来基本没用，只是存储了数据，但数据基本没用起来
    /// </summary>
    public virtual LogisticsModeType FirstType { get; set; } = LogisticsModeType.None;

    /// <summary>
    /// 头程物流
    /// 这个字段后来也没用起来，用另外一张表来存储了
    /// </summary>
    public virtual LogisticsModeInfo FirstMode { get; set; } = new();

    /// <summary>
    /// 头程物流描述
    /// </summary>
    public virtual string FirstModeDesc { get; set; } = string.Empty;

    /// <summary>
    /// 是否有关联的头程物流
    /// </summary>
    public virtual bool HasFirstMode { get; set; } = false;

    /// <summary>
    /// 有效期，非空
    /// </summary>
    public virtual DateTime Expire { get; set; }

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
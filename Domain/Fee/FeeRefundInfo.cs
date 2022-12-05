using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;

namespace shunda.Domain.Fee;

/// <summary>
/// 订单退赔款
/// </summary>
public class FeeRefundInfo : Entity
{
    /// <summary>
    /// 物流订单
    /// </summary>
    [Required(ErrorMessage = "物流订单不能为空！")]
    public virtual LogisticsOrderInfo Order { get; set; } = new ();

    /// <summary>
    /// 订单编号，非空
    /// </summary>
    public virtual string OrderCode { get; set; } = string.Empty;

    /// <summary>
    /// 会员
    /// </summary>
    [Required(ErrorMessage = "会员不能为空！")]
    public virtual CustomerInfo Customer { get; set; } = new ();

    /// <summary>
    /// 类型
    /// </summary>
    [Required(ErrorMessage = "类型不能为空！")]
    [ModelBinder(typeof(FeeRefundTypeModelBinder))]
    public virtual FeeRefundType Type { get; set; } = FeeRefundType.None;

    [Required(ErrorMessage = "退赔原因不能为空！")]
    public virtual FeeRefundReasonInfo Reason { get; set; } = new ();

    /// <summary>
    /// 退赔款原因
    /// </summary>
    [Required(ErrorMessage = "退赔款原因不能为空！")]
    [MaxLength(300, ErrorMessage = "退赔款原因长度不能大于300！")]
    public virtual string ReasonContent { get; set; } = string.Empty;

    /// <summary>
    /// 退赔金额
    /// </summary>
    [Required(ErrorMessage = "退赔金额不能为空！")]
    public virtual decimal Amount { get; set; }

    /// <summary>
    /// 退赔支付方式
    /// </summary>
    [Required(ErrorMessage = "退赔支付方式不能为空！")]
    [ModelBinder(typeof(FeeRefundPayTypeModelBinder))]
    public virtual FeeRefundPayType PayType { get; set; } = FeeRefundPayType.None;

    /// <summary>
    /// 状态
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [ModelBinder(typeof(FeeRefundStatusModelBinder))]
    public virtual FeeRefundStatus Status { get; set; } = FeeRefundStatus.None;

    /// <summary>
    /// 对应申请
    /// </summary>
    [Required(ErrorMessage = "对应申请不能为空！")]
    public virtual FeeRefundApplyInfo Apply { get; set; } = new ();

    #region Cache

    public virtual string CustomerCodeCache { get; set; } = string.Empty;

    #endregion
}
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Customer;
using shunda.Domain.Order;
using shunda.Domain.Pass;

namespace shunda.Domain.Fee;

/// <summary>
/// 物流订单支付
/// </summary>
public class LogisticsOrderPayInfo : Entity
{
    /// <summary>
    /// 订单
    /// </summary>
    [Required(ErrorMessage = "订单不能为空！")]
    public virtual LogisticsOrderInfo Order { get; set; } = new LogisticsOrderInfo();

    /// <summary>
    /// 客户
    /// </summary>
    [Required(ErrorMessage = "客户不能为空！")]
    public virtual CustomerInfo Customer { get; set; } = new CustomerInfo();

    /// <summary>
    /// 支付类型
    /// </summary>
    [Required(ErrorMessage = "支付类型不能为空！")]
    [ModelBinder(typeof(PayTypeModelBinder))]
    public virtual PayType PayType { get; set; } = PayType.None;

    /// <summary>
    /// 支付订单编号
    /// </summary>
    [Required(ErrorMessage = "支付订单编号不能为空！")]
    [MaxLength(25, ErrorMessage = "支付订单编号长度不能大于25！")]
    public virtual string OutTradeNo { get; set; } = string.Empty;

    /// <summary>
    /// PayPal Payment Id
    /// </summary>
    public virtual string PayPalPaymentId { get; set; } = string.Empty;

    /// <summary>
    /// 支付金额
    /// </summary>
    [Required(ErrorMessage = "支付金额不能为空！")]
    public virtual decimal Amount { get; set; }

    /// <summary>
    /// 操作用户
    /// </summary>
    public virtual UserInfo? User { get; set; }

    /// <summary>
    /// 状态，只使用里面的未支付和已支付
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [ModelBinder(typeof(PayStatusModelBinder))]
    public virtual PayStatus Status { get; set; } = PayStatus.None;

    /// <summary>
    /// 核销原因Id
    /// </summary>
    public virtual int? WriteOffReasonId { get; set; }

    /// <summary>
    /// 核销原因
    /// </summary>
    public virtual string WriteOffReasonContent { get; set; } = string.Empty;
}
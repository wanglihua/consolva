using shunda.Core;
using shunda.Domain.Customer;
using shunda.Domain.Express;

namespace shunda.Domain.Order;

// todo 数据表建好后，使用生成的吧

/// <summary>
/// 快递单退运支付
/// </summary>
public class ExpressReturnPayInfo : Entity
{
    /// <summary>
    /// 快递单，非空
    /// </summary>
    public virtual ExpressInfo Express { get; set; } = new ExpressInfo();

    /// <summary>
    /// 客户，非空
    /// </summary>
    public virtual CustomerInfo Customer { get; set; } = new CustomerInfo();

    /// <summary>
    /// 支付类型，非空
    /// </summary>
    public virtual PayType PayType { get; set; } = PayType.None;

    /// <summary>
    /// 支付订单编号，非空
    /// </summary>
    public virtual string OutTradeNo { get; set; } = string.Empty;

    /// <summary>
    /// PayPal Payment Id，可空
    /// </summary>
    public virtual string PaypalPaymentId { get; set; } = string.Empty;

    /// <summary>
    /// 支付金额，非空
    /// </summary>
    public virtual decimal Amount { get; set; }

    /// <summary>
    /// 状态，非空
    /// </summary>
    public virtual PayStatus Status { get; set; } = PayStatus.None;
}
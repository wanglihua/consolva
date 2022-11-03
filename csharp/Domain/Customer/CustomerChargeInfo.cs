using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户充值记录
/// </summary>
public class CustomerChargeInfo : Entity
{
    /// <summary>
    /// 充值方式
    /// </summary>
    [ModelBinder(typeof(CustomerChargeTypeModelBinder))]
    public virtual CustomerChargeType ChargeType { get; set; } = CustomerChargeType.None;

    /// <summary>
    /// 
    /// </summary>
    public virtual CustomerInfo Customer { get; set; } = new CustomerInfo();

    public virtual decimal Amount { get; set; }

    public virtual decimal Award { get; set; }

    public virtual string OutTradeNo { get; set; } = string.Empty;

    public virtual string PayPalPaymentId { get; set; } = string.Empty;

    public virtual string ManualChargeReason { get; set; } = string.Empty;

    public virtual UserInfo? User { get; set; }

    [ModelBinder(typeof(CustomerChargeStatusModelBinder))]
    public virtual CustomerChargeStatus Status { get; set; } = CustomerChargeStatus.None;
}
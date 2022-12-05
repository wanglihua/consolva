using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Pass;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户账户充值
/// </summary>
public class CustomerChargeInfo : Entity
{
    /// <summary>
    /// 充值方式，非空
    /// </summary>
    [Required(ErrorMessage = "充值方式不能为空！")]
    [JsonProperty("chargeType")]
    [ModelBinder(typeof(CustomerChargeTypeModelBinder))]
    public virtual CustomerChargeType ChargeType { get; set; } = CustomerChargeType.None;

    /// <summary>
    /// 客户id，非空
    /// </summary>
    [Required(ErrorMessage = "客户id不能为空！")]
    [JsonProperty("customer")]
    public virtual CustomerInfo Customer { get; set; } = new ();

    /// <summary>
    /// 充值金额，非空
    /// </summary>
    [Required(ErrorMessage = "充值金额不能为空！")]
    [JsonProperty("amount")]
    public virtual decimal Amount { get; set; } = 0m;

    /// <summary>
    /// 奖励金额，非空
    /// </summary>
    [Required(ErrorMessage = "奖励金额不能为空！")]
    [JsonProperty("award")]
    public virtual decimal Award { get; set; } = 0m;

    /// <summary>
    /// 支付订单编号，非空
    /// </summary>
    [Required(ErrorMessage = "支付订单编号不能为空！")]
    [MaxLength(50, ErrorMessage = "支付订单编号长度不能大于50！")]
    [JsonProperty("outTradeNo")]
    public virtual string OutTradeNo { get; set; } = string.Empty;

    /// <summary>
    /// PayPal payment id，非空
    /// </summary>
    [Required(ErrorMessage = "PayPal payment id不能为空！")]
    [MaxLength(50, ErrorMessage = "PayPal payment id长度不能大于50！")]
    [JsonProperty("paypalPaymentId")]
    public virtual string PaypalPaymentId { get; set; } = string.Empty;

    /// <summary>
    /// 手动充值原因，非空
    /// </summary>
    [Required(ErrorMessage = "手动充值原因不能为空！")]
    [MaxLength(300, ErrorMessage = "手动充值原因长度不能大于300！")]
    [JsonProperty("manualChargeReason")]
    public virtual string ManualChargeReason { get; set; } = string.Empty;

    /// <summary>
    /// 操作用户，可空
    /// </summary>
    [JsonProperty("user")]
    public virtual UserInfo? User { get; set; } = null;

    /// <summary>
    /// 充值状态，非空
    /// </summary>
    [Required(ErrorMessage = "充值状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(CustomerChargeStatusModelBinder))]
    public virtual CustomerChargeStatus Status { get; set; } = CustomerChargeStatus.None;

    #region cache

    public virtual string CustomerCodeCache { get; set; } = string.Empty;

    #endregion
}
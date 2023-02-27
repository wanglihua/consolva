using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Customer;
using shunda.Domain.Express;

namespace shunda.Domain.Fee;

/// <summary>
/// 快递单退运支付
/// </summary>
public class ExpressReturnPayInfo : Entity
{
    /// <summary>
    /// 快递单，非空
    /// </summary>
    [Required(ErrorMessage = "快递单不能为空！")]
    [JsonProperty("express")]
    public virtual ExpressInfo Express { get; set; } = new();

    /// <summary>
    /// 客户，非空
    /// </summary>
    [Required(ErrorMessage = "客户不能为空！")]
    [JsonProperty("customer")]
    public virtual CustomerInfo Customer { get; set; } = new();

    /// <summary>
    /// 支付类型，非空
    /// </summary>
    [Required(ErrorMessage = "支付类型不能为空！")]
    [JsonProperty("payType")]
    [ModelBinder(typeof(PayTypeModelBinder))]
    public virtual PayType PayType { get; set; } = PayType.None;

    /// <summary>
    /// 支付单号，非空
    /// </summary>
    [Required(ErrorMessage = "支付单号不能为空！")]
    [MaxLength(50, ErrorMessage = "支付单号长度不能大于50！")]
    [JsonProperty("outTradeNo")]
    public virtual string OutTradeNo { get; set; } = string.Empty;

    /// <summary>
    /// PayPal Payment Id，非空
    /// </summary>
    [Required(ErrorMessage = "PayPal Payment Id不能为空！")]
    [MaxLength(50, ErrorMessage = "PayPal Payment Id长度不能大于50！")]
    [JsonProperty("paypalPaymentId")]
    public virtual string PaypalPaymentId { get; set; } = string.Empty;

    /// <summary>
    /// 支付金额，非空
    /// </summary>
    [Required(ErrorMessage = "支付金额不能为空！")]
    [JsonProperty("amount")]
    public virtual decimal Amount { get; set; } = 0m;

    /// <summary>
    /// 状态，非空
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(PayStatusModelBinder))]
    public virtual PayStatus Status { get; set; } = PayStatus.None;


    #region Cache

    public virtual string CustomerCodeCache { get; set; } = string.Empty;

    #endregion
}
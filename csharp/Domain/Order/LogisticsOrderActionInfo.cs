using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.Pass;

namespace shunda.Domain.Order;

/// <summary>
/// 订单操作日志
/// </summary>
public class LogisticsOrderActionInfo : Entity
{
    /// <summary>
    /// 订单，非空
    /// </summary>
    [Required(ErrorMessage = "订单不能为空！")]
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo Order { get; set; } = new LogisticsOrderInfo();

    /// <summary>
    /// 操作人，可空
    /// </summary>
    [JsonProperty("user")]
    public virtual UserInfo? User { get; set; } = null;

    /// <summary>
    /// 操作客户，可空
    /// </summary>
    [JsonProperty("customer")]
    public virtual CustomerInfo? Customer { get; set; } = null;

    /// <summary>
    /// 操作类型，非空
    /// </summary>
    [Required(ErrorMessage = "操作类型不能为空！")]
    [JsonProperty("type")]
    [ModelBinder(typeof(LogisticsOrderActionTypeModelBinder))]
    public virtual LogisticsOrderActionType Type { get; set; } = LogisticsOrderActionType.None;

    /// <summary>
    /// 订单状态前，非空
    /// </summary>
    [Required(ErrorMessage = "订单状态前不能为空！")]
    [JsonProperty("orderStatusBefore")]
    [ModelBinder(typeof(LogisticsOrderStatusModelBinder))]
    public virtual LogisticsOrderStatus OrderStatusBefore { get; set; } = LogisticsOrderStatus.None;

    /// <summary>
    /// 订单状态后，非空
    /// </summary>
    [Required(ErrorMessage = "订单状态后不能为空！")]
    [JsonProperty("orderStatusAfter")]
    [ModelBinder(typeof(LogisticsOrderStatusModelBinder))]
    public virtual LogisticsOrderStatus OrderStatusAfter { get; set; } = LogisticsOrderStatus.None;

    /// <summary>
    /// 支付状态前，非空
    /// </summary>
    [Required(ErrorMessage = "支付状态前不能为空！")]
    [JsonProperty("payStatusBefore")]
    [ModelBinder(typeof(PayStatusModelBinder))]
    public virtual PayStatus PayStatusBefore { get; set; } = PayStatus.None;

    /// <summary>
    /// 支付状态后，非空
    /// </summary>
    [Required(ErrorMessage = "支付状态后不能为空！")]
    [JsonProperty("payStatusAfter")]
    [ModelBinder(typeof(PayStatusModelBinder))]
    public virtual PayStatus PayStatusAfter { get; set; } = PayStatus.None;

    /// <summary>
    /// 操作备注，非空
    /// </summary>
    [Required(ErrorMessage = "操作备注不能为空！")]
    [MaxLength(800, ErrorMessage = "操作备注长度不能大于800！")]
    [JsonProperty("actionDesc")]
    public virtual string ActionDesc { get; set; } = string.Empty;

}
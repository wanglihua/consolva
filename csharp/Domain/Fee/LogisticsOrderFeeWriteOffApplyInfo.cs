using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Customer;
using shunda.Domain.Order;

namespace shunda.Domain.Fee;

/// <summary>
/// 订单费用核销申请
/// </summary>
public class LogisticsOrderFeeWriteOffApplyInfo : Entity
{
    /// <summary>
    /// 客户，非空
    /// </summary>
    [Required(ErrorMessage = "客户不能为空！")]
    [JsonProperty("customer")]
    public virtual CustomerInfo Customer { get; set; } = new CustomerInfo();

    /// <summary>
    /// 订单，非空
    /// </summary>
    [Required(ErrorMessage = "订单不能为空！")]
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo Order { get; set; } = new LogisticsOrderInfo();

    /// <summary>
    /// 金额，非空
    /// </summary>
    [Required(ErrorMessage = "金额不能为空！")]
    [JsonProperty("amount")]
    public virtual decimal Amount { get; set; } = 0m;

    /// <summary>
    /// 描述，可空
    /// </summary>
    [MaxLength(255, ErrorMessage = "描述长度不能大于255！")]
    [JsonProperty("description")]
    public virtual string? Description { get; set; } = null;

    /// <summary>
    /// 核销原因Id，非空
    /// </summary>
    [Required(ErrorMessage = "核销原因Id不能为空！")]
    [JsonProperty("writeOffReason")]
    public virtual ManualFeeReasonInfo WriteOffReason { get; set; } = new ManualFeeReasonInfo();

    /// <summary>
    /// 核销原因，非空
    /// </summary>
    [Required(ErrorMessage = "核销原因不能为空！")]
    [MaxLength(300, ErrorMessage = "核销原因长度不能大于300！")]
    [JsonProperty("writeOffReasonContent")]
    public virtual string WriteOffReasonContent { get; set; } = string.Empty;

    /// <summary>
    /// 申请用户，非空
    /// </summary>
    [Required(ErrorMessage = "申请用户不能为空！")]
    [JsonProperty("applyUserId")]
    public virtual int ApplyUserId { get; set; } = 0;

    /// <summary>
    /// 审核用户，可空
    /// </summary>
    [JsonProperty("approveUserId")]
    public virtual int? ApproveUserId { get; set; } = null;

    /// <summary>
    /// 审核状态，非空
    /// </summary>
    [Required(ErrorMessage = "审核状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(LogisticsOrderFeeWriteOffApplyStatusModelBinder))]
    public virtual LogisticsOrderFeeWriteOffApplyStatus Status { get; set; } = LogisticsOrderFeeWriteOffApplyStatus.None;

}
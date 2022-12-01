using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;

namespace shunda.Domain.Fee;

/// <summary>
/// 订单手工费用申请
/// </summary>
public class ManualFeeApplyInfo : Entity
{
    /// <summary>
    ///  客户，非空
    /// </summary>
    [Required(ErrorMessage = " 客户不能为空！")]
    [JsonProperty("customer")]
    public virtual CustomerInfo Customer { get; set; } = new CustomerInfo();

    /// <summary>
    /// 订单，非空
    /// </summary>
    [Required(ErrorMessage = "订单不能为空！")]
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo Order { get; set; } = new LogisticsOrderInfo();

    /// <summary>
    /// 订单编号，非空
    /// </summary>
    public virtual string OrderCode { get; set; } = string.Empty;

    /// <summary>
    /// 费用类型，非空
    /// </summary>
    [Required(ErrorMessage = "费用类型不能为空！")]
    [JsonProperty("type")]
    [ModelBinder(typeof(LogisticsOrderFeeTypeModelBinder))]
    public virtual LogisticsOrderFeeType Type { get; set; } = LogisticsOrderFeeType.None;

    /// <summary>
    /// 金额，非空
    /// </summary>
    [Required(ErrorMessage = "金额不能为空！")]
    [JsonProperty("fee")]
    public virtual decimal Fee { get; set; } = 0m;

    /// <summary>
    /// 描述，非空
    /// </summary>
    [Required(ErrorMessage = "描述不能为空！")]
    [MaxLength(255, ErrorMessage = "描述长度不能大于255！")]
    [JsonProperty("description")]
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// 增减核销费用原因id，非空
    /// </summary>
    [Required(ErrorMessage = "增减核销费用原因id不能为空！")]
    [JsonProperty("manualReason")]
    public virtual ManualFeeReasonInfo ManualReason { get; set; } = new ManualFeeReasonInfo();

    /// <summary>
    /// 增减核销费用原因content，非空
    /// </summary>
    [Required(ErrorMessage = "增减核销费用原因content不能为空！")]
    [MaxLength(300, ErrorMessage = "增减核销费用原因content长度不能大于300！")]
    [JsonProperty("manualReasonContent")]
    public virtual string ManualReasonContent { get; set; } = string.Empty;

    /// <summary>
    /// 申请用户，非空
    /// </summary>
    [Required(ErrorMessage = "申请用户不能为空！")]
    [JsonProperty("applyUser")]
    public virtual UserInfo ApplyUser { get; set; } = new UserInfo();

    /// <summary>
    /// 审核用户，可空
    /// </summary>
    [JsonProperty("approveUser")]
    public virtual UserInfo? ApproveUser { get; set; } = null;

    /// <summary>
    /// 审核状态，非空
    /// </summary>
    [Required(ErrorMessage = "审核状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(ManualFeeApplyStatusModelBinder))]
    public virtual ManualFeeApplyStatus Status { get; set; } = ManualFeeApplyStatus.None;

    #region Cache

    public virtual int OrderApplyCountCache { get; set; }

    public virtual string CustomerCodeCache { get; set; } = string.Empty;

    #endregion
}
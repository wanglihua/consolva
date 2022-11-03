using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Core;

namespace shunda.Domain.Customer;

/// <summary>
/// 手动充值原因
/// </summary>
public class ManualChargeReasonInfo : Entity
{
    /// <summary>
    /// 内容
    /// </summary>
    [DisplayName("内容")]
    [Required(ErrorMessage = "内容不能为空！")]
    [MaxLength(300, ErrorMessage = "内容长度不能大于300！")]
    public virtual string Content { get; set; } = string.Empty;

    /// <summary>
    /// 状态
    /// </summary>
    [DisplayName("状态")]
    [Required(ErrorMessage = "状态不能为空！")]
    [ModelBinder(typeof(ManualChargeReasonStatusModelBinder))]
    public virtual ManualChargeReasonStatus Status { get; set; } = ManualChargeReasonStatus.None;
}
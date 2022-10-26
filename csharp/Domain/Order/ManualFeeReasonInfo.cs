using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Core;

namespace shunda.Domain.Order;

/// <summary>
/// 增减核销费用原因
/// </summary>
public class ManualFeeReasonInfo : Entity
{
    /// <summary>
    /// 费用类型
    /// </summary>
    [Required(ErrorMessage = "费用类型不能为空！")]
    [ModelBinder(typeof(ManualFeeTypeModelBinder))]
    public virtual ManualFeeType FeeType { get; set; } = ManualFeeType.None;

    /// <summary>
    /// 内容
    /// </summary>
    [Required(ErrorMessage = "内容不能为空！")]
    [MaxLength(300, ErrorMessage = "内容长度不能大于300！")]
    public virtual string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// 状态
    /// </summary>
    [Required(ErrorMessage = "状态不能为空!")]
    [ModelBinder(typeof(ManualFeeReasonStatusModelBinder))]
    public virtual ManualFeeReasonStatus Status { get; set; } = ManualFeeReasonStatus.None;
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Core;

namespace shunda.Domain.Fee;

/// <summary>
/// 退赔原因
/// </summary>
public class FeeRefundReasonInfo : Entity
{
    /// <summary>
    /// 内容
    /// </summary>
    [Required(ErrorMessage = "内容不能为空！")]
    [MaxLength(300, ErrorMessage = "内容长度不能大于300！")]
    public virtual string Content { get; set; } = string.Empty;

    /// <summary>
    /// 状态
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [ModelBinder(typeof(FeeRefundReasonStatusModelBinder))]
    public virtual FeeRefundReasonStatus Status { get; set; } = FeeRefundReasonStatus.None;
}
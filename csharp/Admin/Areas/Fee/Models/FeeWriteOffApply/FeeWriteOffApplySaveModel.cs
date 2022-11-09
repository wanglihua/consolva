using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeWriteOffApply;

public class FeeWriteOffApplySaveModel
{
    public int ApplyId { get; set; }

    [Required(ErrorMessage = "订单编号不能为空！")]
    [MaxLength(100, ErrorMessage = "订单编号长度不能大于100！")]
    public string OrderCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "客户ID不能为空！")]
    [MaxLength(20, ErrorMessage = "客户ID长度不能大于20！")]
    public string CustomerCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "已付金额不能为空！")]
    public decimal? Amount { get; set; }

    [Required(ErrorMessage = "核销原因不能为空！")]
    public int FeeReasonId { get; set; }

    [MaxLength(255, ErrorMessage = "描述长度不能大于255！")]
    public string? Description { get; set; }
}
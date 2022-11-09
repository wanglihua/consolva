using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeApply;

public class ManualFeeApplySaveModel
{
    public int ApplyId { get; set; }

    /// <summary>
    /// 订单编号
    /// </summary>
    [Required(ErrorMessage = "订单编号不能为空！")]
    [MaxLength(100, ErrorMessage = "订单编号长度不能大于100！")]
    public string OrderCode { get; set; } = String.Empty;

    /// <summary>
    /// 客户编号
    /// </summary>
    [Required(ErrorMessage = "客户ID不能为空！")]
    public string CustomerCode { get; set; } = string.Empty;

    /// <summary>
    /// 金额
    /// </summary>
    public decimal Fee { get; set; }

    /// <summary>
    /// 增减核销费用原因
    /// </summary>
    [Required(ErrorMessage = "减少费用原因不能为空！")]
    public int ManualReasonId { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(255, ErrorMessage = "描述长度不能大于255！")]
    public string? Description { get; set; }
}
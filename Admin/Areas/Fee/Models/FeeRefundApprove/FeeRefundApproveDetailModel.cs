using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeRefundApprove;

public class FeeRefundApproveDetailModel
{
    [BindNever]
    public FeeRefundApplyInfo FeeRefundApply { get; set; } = new FeeRefundApplyInfo();

    [Required(ErrorMessage = "退赔款申请单不能为空！")]
    public int ApplyId { get; set; }

    /// <summary>
    /// 审核备注
    /// </summary>
    [MaxLength(300, ErrorMessage = "审核备注长度不能大于300！")]
    public string ApproveMemo { get; set; } = string.Empty;
}
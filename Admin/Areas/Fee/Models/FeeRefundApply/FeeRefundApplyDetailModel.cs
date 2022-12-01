using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeRefundApply;

public class FeeRefundApplyDetailModel : FeeRefundApplySaveModel
{
    [BindNever]
    public IList<FeeRefundType> TypeList { get; set; } = FeeRefundType.GetList();

    [BindNever]
    public IList<FeeRefundReasonInfo> ReasonList { get; set; } = new List<FeeRefundReasonInfo>();
}
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeReason;

public class ManualFeeReasonDetailModel
{
    public ManualFeeReasonInfo ManualFeeReason { get; set; } = new();

    [BindNever]
    public IList<ManualFeeType> FeeTypeList { get; set; } = ManualFeeType.GetList();

    [BindNever]
    public IList<ManualFeeReasonStatus> StatusList { get; set; } = ManualFeeReasonStatus.GetList();
}
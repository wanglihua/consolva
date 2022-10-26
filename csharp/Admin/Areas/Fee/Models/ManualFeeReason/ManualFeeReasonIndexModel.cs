using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Order;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeReason;

public class ManualFeeReasonIndexModel
{
    [BindNever]
    public IList<ManualFeeType> FeeTypeList { get; set; } = ManualFeeType.GetList();

    [BindNever]
    public IList<ManualFeeReasonStatus> StatusList { get; set; } = ManualFeeReasonStatus.GetList();
}
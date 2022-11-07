using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;
using shunda.Domain.Order;

namespace shunda.Admin.Areas.OrderFee.Models.ManualFeeReason;

public class ManualFeeReasonIndexModel
{
    [BindNever]
    public IList<ManualFeeType> FeeTypeList { get; set; } = ManualFeeType.GetList();

    [BindNever]
    public IList<ManualFeeReasonStatus> StatusList { get; set; } = ManualFeeReasonStatus.GetList();
}
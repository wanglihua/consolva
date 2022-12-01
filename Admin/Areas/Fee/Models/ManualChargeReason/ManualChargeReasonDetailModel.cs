using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Fee.Models.ManualChargeReason;

public class ManualChargeReasonDetailModel
{
    public ManualChargeReasonInfo ManualChargeReason { get; set; } = new();

    [BindNever]
    public IList<ManualChargeReasonStatus> ReasonStatusList { get; set; } = ManualChargeReasonStatus.GetList();
}
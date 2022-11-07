using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.OrderFee.Models.ManualChargeReason;

public class ManualChargeReasonDetailModel
{
    public ManualChargeReasonInfo ManualChargeReason { get; set; } = new ManualChargeReasonInfo();

    [BindNever]
    public IList<ManualChargeReasonStatus> ReasonStatusList { get; set; } = ManualChargeReasonStatus.GetList();
}
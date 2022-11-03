using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.OrderFee.Models.CustomerCharge;

public class CustomerChargeIndexModel
{
    [BindNever]
    public IList<CustomerChargeType> ChargeTypeList { get; set; } = CustomerChargeType.GetList();

    [BindNever]
    public bool HasManualChargePass { get; set; }
}
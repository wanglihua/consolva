using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Fee.Models.CustomerCharge;

public class CustomerChargeIndexModel
{
    [BindNever]
    public IList<CustomerChargeType> ChargeTypeList { get; set; } = CustomerChargeType.GetList();

    [BindNever]
    public bool HasManualChargePass { get; set; }
}
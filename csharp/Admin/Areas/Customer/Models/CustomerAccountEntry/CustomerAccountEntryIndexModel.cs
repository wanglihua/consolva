using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerAccountEntry;

public class CustomerAccountEntryIndexModel
{
    [BindNever]
    public CustomerInfo Customer { get; set; } = new();

    [BindNever]
    public IList<CustomerAccountChangeType> ChangeTypeList { get; set; } = new List<CustomerAccountChangeType>();
}
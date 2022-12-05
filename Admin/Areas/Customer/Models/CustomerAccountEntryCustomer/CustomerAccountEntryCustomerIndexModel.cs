using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerAccountEntryCustomer;

public class CustomerAccountEntryCustomerIndexModel
{
    public CustomerInfo Customer { get; set; } = new ();
}
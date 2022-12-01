using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerAccountEntryCustomer;

public class CustomerAccountEntryCustomerListQueryParam : ListQueryParam
{
    public int CustomerId { get; set; }

    [ModelBinder(typeof(CustomerAccountChangeTypeModelBinder))]
    public CustomerAccountChangeType? ChangeTypeQuery { get; set; }
}
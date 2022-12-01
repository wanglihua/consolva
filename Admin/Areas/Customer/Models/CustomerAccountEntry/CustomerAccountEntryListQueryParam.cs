using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerAccountEntry;

public class CustomerAccountEntryListQueryParam : ListQueryParam
{
    public string? CustomerCodeQuery { get; set; }

    [ModelBinder(typeof(CustomerAccountChangeTypeModelBinder))]
    public CustomerAccountChangeType? ChangeTypeQuery { get; set; }
}
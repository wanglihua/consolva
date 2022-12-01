using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.Customer;

public class CustomerListQueryParam : ListQueryParam
{
    public string? CustomerCodeQuery { get; set; }

    public int? VipLevelQuery { get; set; }

    [ModelBinder(typeof(CustomerStatusModelBinder))]
    public CustomerStatus? StatusQuery { get; set; }

    public DateTime? BeginDateQuery { get; set; }

    public DateTime? EndDateQuery { get; set; }

    public string? CharacterQuery { get; set; }
}
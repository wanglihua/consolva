using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.OrderFee.Models.CustomerCharge;

public class CustomerChargeListQueryParam : ListQueryParam
{
    public string? CustomerCodeQuery { get; set; }

    [ModelBinder(typeof(CustomerChargeTypeModelBinder))]
    public CustomerChargeType? ChargeTypeQuery { get; set; }
}
using shunda.Core;

namespace shunda.Admin.Areas.Customer.Models.CustomerSuperVipDiscount;

public class CustomerSuperVipDiscountListQueryParam : ListQueryParam
{
    public string? CustomerCode { get; set; }
}
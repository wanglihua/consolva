using shunda.Core;

namespace shunda.Admin.Areas.Customer.Models.LogisticsOrderPayCustomer;

public class LogisticsOrderPayCustomerListQueryParam : ListQueryParam
{
    public int CustomerId { get; set; }

    public string? OrderCodeQuery { get; set; }
}
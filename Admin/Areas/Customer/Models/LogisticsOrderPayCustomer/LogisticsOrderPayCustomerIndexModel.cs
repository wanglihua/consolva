using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.LogisticsOrderPayCustomer;

public class LogisticsOrderPayCustomerIndexModel
{
    [BindNever]
    public int CustomerId { get; set; }

    [BindNever]
    public CustomerInfo Customer { get; set; } = new ();
}
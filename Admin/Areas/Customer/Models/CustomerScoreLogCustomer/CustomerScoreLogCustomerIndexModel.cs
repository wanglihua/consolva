using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerScoreLogCustomer;

public class CustomerScoreLogCustomerIndexModel
{
    [BindNever]
    public int CustomerId { get; set; }

    [BindNever]
    public CustomerInfo Customer { get; set; } = new ();

    [BindNever]
    public IList<CustomerScoreType> ScoreTypeList { get; set; } = CustomerScoreType.GetList();
}
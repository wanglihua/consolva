using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.Customer;

public class CustomerListModel : ListModel
{
    public CustomerListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<CustomerInfo> CustomerList { get; set; } = new List<CustomerInfo>();
}
using shunda.Core;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerAccountEntryCustomer;

public class CustomerAccountEntryCustomerListModel : ListModel
{
    public CustomerAccountEntryCustomerListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<CustomerAccountEntryInfo> CustomerAccountEntryList { get; set; } = new List<CustomerAccountEntryInfo>();
}
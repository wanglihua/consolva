using shunda.Core;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerAccountEntry;

public class CustomerAccountEntryListModel : ListModel
{
    public CustomerAccountEntryListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<CustomerAccountEntryInfo> CustomerAccountEntryList { get; set; } = new List<CustomerAccountEntryInfo>();
}
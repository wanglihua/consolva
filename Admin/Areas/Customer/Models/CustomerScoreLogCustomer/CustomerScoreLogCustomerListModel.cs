using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerScoreLogCustomer;

public class CustomerScoreLogCustomerListModel : ListModel
{
    public CustomerScoreLogCustomerListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<CustomerScoreLogInfo> CustomerScoreLogList { get; set; } = new List<CustomerScoreLogInfo>();
}
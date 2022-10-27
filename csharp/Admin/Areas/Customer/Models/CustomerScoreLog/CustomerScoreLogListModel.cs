using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerScoreLog;

public class CustomerScoreLogListModel : ListModel
{
    public CustomerScoreLogListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<CustomerScoreLogInfo> CustomerScoreLogList { get; set; } = new List<CustomerScoreLogInfo>();
}
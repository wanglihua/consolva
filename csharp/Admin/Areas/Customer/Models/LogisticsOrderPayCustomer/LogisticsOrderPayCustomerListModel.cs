using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Customer.Models.LogisticsOrderPayCustomer;

public class LogisticsOrderPayCustomerListModel : ListModel
{
    public LogisticsOrderPayCustomerListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<LogisticsOrderPayInfo> LogisticsOrderPayList { get; set; } = new List<LogisticsOrderPayInfo>();
}
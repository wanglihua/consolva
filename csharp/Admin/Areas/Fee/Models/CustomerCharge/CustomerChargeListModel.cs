using shunda.Core;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.OrderFee.Models.CustomerCharge;

public class CustomerChargeListModel : ListModel
{
    public CustomerChargeListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<CustomerChargeInfo> CustomerChargeList { get; set; } = new List<CustomerChargeInfo>();
}
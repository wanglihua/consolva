using shunda.Core;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerVipDiscount;

public class CustomerVipDiscountListModel : ListModel
{
    public CustomerVipDiscountListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<CustomerVipDiscountInfo> CustomerVipDiscountList { get; set; } = new List<CustomerVipDiscountInfo>();
}
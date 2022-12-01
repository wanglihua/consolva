using shunda.Core;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerSuperVipDiscount;

public class CustomerSuperVipDiscountListModel : ListModel
{
    public CustomerSuperVipDiscountListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<CustomerSuperVipDiscountInfo> CustomerSuperVipDiscountList { get; set; } = new List<CustomerSuperVipDiscountInfo>();
}
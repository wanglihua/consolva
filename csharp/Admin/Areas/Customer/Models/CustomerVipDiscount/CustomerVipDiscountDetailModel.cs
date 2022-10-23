using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerVipDiscount;

public class CustomerVipDiscountDetailModel
{
    public CustomerVipDiscountInfo CustomerVipDiscount { get; set; } = new CustomerVipDiscountInfo();
}
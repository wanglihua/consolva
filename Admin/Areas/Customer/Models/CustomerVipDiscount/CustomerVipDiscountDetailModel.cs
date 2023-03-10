using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerVipDiscount;

public class CustomerVipDiscountDetailModel : CustomerVipDiscountSaveModel {
    [BindNever]
    public bool IsCreate { get; set; }

    [BindNever]
    public IList<CustomerVipRank> CustomerVipRankList { get; set; } = CustomerVipRank.GetList();
}
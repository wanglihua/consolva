using shunda.Core;
using shunda.Domain.Customer;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Customer.Models.CustomerPromotion;

public class CustomerPromotionListModel : ListModel {
    public CustomerPromotionListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public string CustomerPromotionAlias { get; set; } = string.Empty;

    [BindNever]
    public string ParentAlias { get; set; } = string.Empty;

    [BindNever]
    public string ChildAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<CustomerPromotionInfo> CustomerPromotionList { get; set; } = new List<CustomerPromotionInfo>();
}
using shunda.Core;
using shunda.Domain.Customer;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Customer.Models.CustomerPromotionImage;

public class CustomerPromotionImageListModel : ListModel {
    public CustomerPromotionImageListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public string CustomerPromotionImageAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<CustomerPromotionImageInfo> CustomerPromotionImageList { get; set; } = new List<CustomerPromotionImageInfo>();
}
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerPromotionImage;

public class CustomerPromotionImageIndexModel {
    [BindNever]
    public IList<CustomerPromotionImageInfo> CustomerPromotionImageList { get; set; } = new List<CustomerPromotionImageInfo>();

    [BindNever]
    public bool PromotionClientShow { get; set; }

    [BindNever]
    public string PromotionImageDesc { get; set; } = string.Empty;
}
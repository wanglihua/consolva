using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerPromotion;

public class CustomerPromotionModel {
    [JsonProperty("promotionImageDesc")]
    public string PromotionImageDesc { get; set; } = string.Empty;

    [JsonProperty("promotionImageList")]
    public IList<CustomerPromotionImageItem> PromotionImageList { get; set; } = new List<CustomerPromotionImageItem>();
}
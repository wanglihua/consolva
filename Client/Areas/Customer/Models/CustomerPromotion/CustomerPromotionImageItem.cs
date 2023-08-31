using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerPromotion;

public class CustomerPromotionImageItem {
    [JsonProperty("imageNum")]
    public int ImageNum { get; set; }

    [JsonProperty("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonProperty("smallImageUrl")]
    public string SmallImageUrl { get; set; } = string.Empty;
}
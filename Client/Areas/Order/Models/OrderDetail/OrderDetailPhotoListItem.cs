using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailPhotoListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("smallUrl")]
    public string SmallUrl { get; set; } = string.Empty;

    [JsonProperty("bigUrl")]
    public string BigUrl { get; set; } = string.Empty;
}
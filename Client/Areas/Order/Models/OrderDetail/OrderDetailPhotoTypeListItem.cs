using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailPhotoTypeListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("photoCount")]
    public int PhotoCount { get; set; }

    [JsonProperty("photoList")]
    public IList<OrderDetailPhotoListItem> PhotoList = new List<OrderDetailPhotoListItem>();
}
using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailOrderPackItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("firstModeName")]
    public string FirstModeName { get; set; } = string.Empty;

    [JsonProperty("goodsType")]
    public string GoodsType { get; set; } = string.Empty;

    [JsonProperty("singleSize")]
    public string SingleSize { get; set; } = string.Empty;

    [JsonProperty("singleVolume")]
    public string SingleVolume { get; set; } = string.Empty;

    [JsonProperty("singleWeight")]
    public string SingleWeight { get; set; } = string.Empty;

    [JsonProperty("packageCount")]
    public int PackageCount { get; set; }
}
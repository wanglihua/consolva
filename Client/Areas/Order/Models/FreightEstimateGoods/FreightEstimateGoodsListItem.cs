using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.FreightEstimateGoods;

public class FreightEstimateGoodsListItem {
    [JsonProperty("width")]
    public string Width { get; set; } = string.Empty;

    [JsonProperty("length")]
    public string Length { get; set; } = string.Empty;

    [JsonProperty("height")]
    public string Height { get; set; } = string.Empty;

    [JsonProperty("weight")]
    public string Weight { get; set; } = string.Empty;

    [JsonProperty("pacakgeCount")]
    public string PacakgeCount { get; set; } = string.Empty;
}
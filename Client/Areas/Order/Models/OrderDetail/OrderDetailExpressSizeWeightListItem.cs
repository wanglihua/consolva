using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailExpressSizeWeightListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("trackingNo")]
    public string TrackingNo { get; set; } = string.Empty;

    [JsonProperty("length")]
    public string Length { get; set; } = string.Empty;

    [JsonProperty("width")]
    public string Width { get; set; } = string.Empty;

    [JsonProperty("height")]
    public string Height { get; set; } = string.Empty;

    [JsonProperty("volumeWeight")]
    public string VolumeWeight { get; set; } = string.Empty;

    [JsonProperty("actualWeight")]
    public string ActualWeight { get; set; } = string.Empty;

    [JsonProperty("packageCount")]
    public int PackageCount { get; set; }
}
using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.FreightEstimateSummary; 

public class FreightEstimateSummaryGoodsListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("width")]
    public string Width { get; set; } = string.Empty;

    [JsonProperty("length")]
    public string Length { get; set; } = string.Empty;

    [JsonProperty("height")]
    public string Height { get; set; } = string.Empty;

    [JsonProperty("weight")]
    public string Weight { get; set; } = string.Empty;

    [JsonProperty("packageCount")]
    public string PackageCount { get; set; } = string.Empty;
}
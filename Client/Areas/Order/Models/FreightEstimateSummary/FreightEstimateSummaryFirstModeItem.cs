using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.FreightEstimateSummary;

public class FreightEstimateSummaryFirstModeItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; } = string.Empty;

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
}
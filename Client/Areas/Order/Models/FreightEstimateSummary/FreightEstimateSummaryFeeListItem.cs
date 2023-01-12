using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.FreightEstimateSummary;

public class FreightEstimateSummaryFeeListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("fee")]
    public string Fee { get; set; } = string.Empty;
}
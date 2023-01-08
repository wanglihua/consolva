using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderCreateSummary;

public class OrderCreateSummaryGstItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}
using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.OrderCreateSummary;

public class OrderCreateSummaryRepackTypeItem {
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
}
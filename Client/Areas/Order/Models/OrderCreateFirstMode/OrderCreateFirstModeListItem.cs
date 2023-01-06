using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderCreateFirstMode;

public class OrderCreateFirstModeListItem {
    [JsonProperty("modeId")]
    public int ModeId { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; } = string.Empty;

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("modeName")]
    public string ModeName { get; set; } = string.Empty;

    [JsonProperty("modeDesc")]
    public string ModeDesc { get; set; } = string.Empty;
}
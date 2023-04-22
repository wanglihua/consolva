using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderLogisticsTrack;

public class OrderLogisticsTrackEventItem {
    [JsonProperty("eventTime")]
    public string EventTime { get; set; } = string.Empty;

    [JsonProperty("eventDesc")]
    public string EventDesc { get; set; } = string.Empty;
}
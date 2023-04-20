using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack.SeventeenTrack.Register;

public class SeventeenTrackRegisterRejectedItem {
    [JsonProperty("number")]
    public string Number { get; set; } = string.Empty;

    [JsonProperty("tag")]
    public string Tag { get; set; } = string.Empty;

    [JsonProperty("error")]
    public SeventeenTrackRegisterRejectedItemError Error { get; set; } = new();
}
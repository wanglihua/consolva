using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack.SeventeenTrack.Register;

public class SeventeenTrackRegisterAcceptedItem {
    [JsonProperty("origin")]
    public int Origin { get; set; }

    [JsonProperty("number")]
    public string Number { get; set; } = string.Empty;

    [JsonProperty("tag")]
    public string Tag { get; set; } = string.Empty;

    [JsonProperty("carrier")]
    public int Carrier { get; set; }
}
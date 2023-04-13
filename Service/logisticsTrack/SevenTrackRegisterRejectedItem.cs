using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack;

public class SevenTrackRegisterRejectedItem {
    [JsonProperty("number")]
    public string Number { get; set; } = string.Empty;

    [JsonProperty("tag")]
    public string Tag { get; set; } = string.Empty;

    [JsonProperty("error")]
    public SevenTrackRegisterRejectedItemError Error { get; set; } = new SevenTrackRegisterRejectedItemError();
}
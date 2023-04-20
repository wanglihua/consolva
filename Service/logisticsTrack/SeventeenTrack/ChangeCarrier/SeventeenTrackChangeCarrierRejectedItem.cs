using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack.SeventeenTrack.ChangeCarrier;

public class SeventeenTrackChangeCarrierRejectedItem {
    [JsonProperty("number")]
    public string Number { get; set; } = string.Empty;

    [JsonProperty("error")]
    public SeventeenTrackChangeCarrierRejectedItemError Error { get; set; } = new();
}
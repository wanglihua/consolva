using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack.SeventeenTrack.ChangeCarrier;

public class SeventeenTrackChangeCarrierAcceptedItem {
    [JsonProperty("number")]
    public string Number { get; set; } = string.Empty;

    [JsonProperty("carrier")]
    public int Carrier { get; set; }
}
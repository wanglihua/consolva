using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack.SeventeenTrack.ChangeCarrier;

public class SeventeenTrackChangeCarrierRequestItem {
    [JsonProperty("number")]
    public string Number { get; set; } = string.Empty;

    [JsonProperty("carrier_old")]
    public int CarrierOld { get; set; }

    [JsonProperty("carrier_new")]
    public int CarrierNew { get; set; }

    [JsonProperty("final_carrier_old")]
    public int FinalCarrierOld { get; set; } = 0;

    [JsonProperty("final_carrier_new")]
    public int FinalCarrierNew { get; set; } = 0;
}
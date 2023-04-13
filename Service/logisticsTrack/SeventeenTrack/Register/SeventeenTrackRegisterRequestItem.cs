using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack.SeventeenTrack.Register;

public class SevenTrackRegisterRequestItem {
    [JsonProperty("number")]
    public string Number { get; set; } = string.Empty;

    [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
    public string? Email { get; set; }

    [JsonProperty("param", NullValueHandling = NullValueHandling.Ignore)]
    public string? Param { get; set; }

    [JsonProperty("carrier", NullValueHandling = NullValueHandling.Ignore)]
    public int? Carrier { get; set; }

    [JsonProperty("final_carrier", NullValueHandling = NullValueHandling.Ignore)]
    public int? FinalCarrier { get; set; }

    [JsonProperty("auto_detection")]
    public bool AutoDetection { get; set; } = true;

    [JsonProperty("tag", NullValueHandling = NullValueHandling.Ignore)]
    public string? Tag { get; set; }
}
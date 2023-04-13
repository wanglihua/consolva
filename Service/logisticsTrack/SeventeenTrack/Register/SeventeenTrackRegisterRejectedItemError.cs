using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack.SeventeenTrack.Register;

public class SevenTrackRegisterRejectedItemError {
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;
}

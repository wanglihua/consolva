using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack.SeventeenTrack;

public class SeventeenTrackResponseDataError {
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;
}

using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack;

public class SevenTrackResponseDataError {
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;
}

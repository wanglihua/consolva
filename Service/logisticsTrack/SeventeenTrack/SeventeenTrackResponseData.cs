using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack.SeventeenTrack;

public class SeventeenTrackResponseData {
    [JsonProperty("errors", NullValueHandling = NullValueHandling.Ignore)]
    public IList<SeventeenTrackResponseDataError>? ErrorList { get; set; }
}
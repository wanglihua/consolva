using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack;

public class SevenTrackResponseData {
    [JsonProperty("errors", NullValueHandling = NullValueHandling.Ignore)]
    public IList<SevenTrackResponseDataError>? ErrorList { get; set; }
}
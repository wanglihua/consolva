using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack;

public class SevenTrackRegisterResponseData {
    [JsonProperty("accepted", NullValueHandling = NullValueHandling.Ignore)]
    public IList<SevenTrackRegisterAcceptedItem>? AcceptedList { get; set; }

    [JsonProperty("rejected", NullValueHandling = NullValueHandling.Ignore)]
    public IList<SevenTrackRegisterRejectedItem>? RejectedList { get; set; }
}
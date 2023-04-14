using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack.SeventeenTrack.Register;

public class SevenTrackRegisterResponseData : SevenTrackResponseData {
    [JsonProperty("accepted", NullValueHandling = NullValueHandling.Ignore)]
    public IList<SeventeenTrackRegisterAcceptedItem>? AcceptedList { get; set; }

    [JsonProperty("rejected", NullValueHandling = NullValueHandling.Ignore)]
    public IList<SevenTrackRegisterRejectedItem>? RejectedList { get; set; }
}
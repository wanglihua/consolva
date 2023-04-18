using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack.SeventeenTrack.Register;

public class SeventeenTrackRegisterResponseData : SeventeenTrackResponseData {
    [JsonProperty("accepted", NullValueHandling = NullValueHandling.Ignore)]
    public IList<SeventeenTrackRegisterAcceptedItem>? AcceptedList { get; set; }

    [JsonProperty("rejected", NullValueHandling = NullValueHandling.Ignore)]
    public IList<SeventeenTrackRegisterRejectedItem>? RejectedList { get; set; }
}
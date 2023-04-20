using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack.SeventeenTrack.ChangeCarrier;

public class SeventeenTrackChangeCarrierResponseData : SeventeenTrackResponseData {
    [JsonProperty("accepted", NullValueHandling = NullValueHandling.Ignore)]
    public IList<SeventeenTrackChangeCarrierAcceptedItem>? AcceptedList { get; set; }

    [JsonProperty("rejected", NullValueHandling = NullValueHandling.Ignore)]
    public IList<SeventeenTrackChangeCarrierRejectedItem>? RejectedList { get; set; }
}
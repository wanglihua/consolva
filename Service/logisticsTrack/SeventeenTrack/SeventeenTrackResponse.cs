using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack.SeventeenTrack;

public class SevenTrackResponse<TData> where TData : SevenTrackResponseData, new() {
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("data")]
    public TData Data { get; set; } = new TData();
}
using shunda.Core;

namespace shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackCarrier;

public class SeventeenTrackCarrierListQueryParam : ListQueryParam {
    public string? CarrierKeyQuery { get; set; }

    public string? CarrierNameQuery { get; set; }
}
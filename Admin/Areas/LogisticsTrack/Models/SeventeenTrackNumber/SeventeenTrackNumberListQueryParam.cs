using shunda.Core;
using shunda.Domain.LogisticsTrack;
using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackNumber;

public class SeventeenTrackNumberListQueryParam : ListQueryParam {
    public string? TrackingNoQuery { get; set; }

    public int? CarrierKeyQuery { get; set; }

    public string? CarrierNameQuery { get; set; }

    [ModelBinder(typeof(SeventeenTrackNumberStatusModelBinder))]
    public SeventeenTrackNumberStatus? StatusQuery { get; set; }
}
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.LogisticsTrack;

namespace shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackNumberEvent;

public class SeventeenTrackNumberEventIndexModel {
    [BindNever]
    public int TrackNumberId { get; set; }

    [BindNever]
    public SeventeenTrackNumberInfo TrackNumber { get; set; } = new SeventeenTrackNumberInfo();
}
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.LogisticsTrack;

namespace shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackNumberEvent;

public class SeventeenTrackNumberEventIndexModel {
    [BindNever]
    public SeventeenTrackNumberInfo TrackNumber { get; set; } = new SeventeenTrackNumberInfo();

    [BindNever]
    public IList<SeventeenTrackNumberEventInfo> SeventeenTrackNumberEventList { get; set; } = new List<SeventeenTrackNumberEventInfo>();
}
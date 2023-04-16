using shunda.Core;
using shunda.Domain.LogisticsTrack;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackNumberEvent;

public class SeventeenTrackNumberEventListModel {
    [BindNever]
    public string SeventeenTrackNumberEventAlias { get; set; } = string.Empty;

    [BindNever]
    public string TrackNumberAlias { get; set; } = string.Empty;

    [BindNever]
    public IList<SeventeenTrackNumberEventInfo> SeventeenTrackNumberEventList { get; set; } = new List<SeventeenTrackNumberEventInfo>();
}
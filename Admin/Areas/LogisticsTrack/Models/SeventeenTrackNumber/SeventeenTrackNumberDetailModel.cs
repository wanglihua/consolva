using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.LogisticsTrack;

namespace shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackNumber;

public class SeventeenTrackNumberDetailModel : SeventeenTrackNumberSaveModel {
    [BindNever]
    public IList<SeventeenTrackNumberOrigin> OriginList { get; set; } = new List<SeventeenTrackNumberOrigin>();

    [BindNever]
    public IList<SeventeenTrackNumberStatus> StatusList { get; set; } = new List<SeventeenTrackNumberStatus>();

}
using shunda.Core;
using shunda.Domain.LogisticsTrack;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackNumber;

public class SeventeenTrackNumberListModel : ListModel {
    public SeventeenTrackNumberListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public string SeventeenTrackNumberAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<SeventeenTrackNumberInfo> SeventeenTrackNumberList { get; set; } = new List<SeventeenTrackNumberInfo>();
}
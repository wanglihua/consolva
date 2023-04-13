using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.LogisticsTrack;

namespace shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackCarrier;

public class SeventeenTrackCarrierListModel : ListModel {
    public SeventeenTrackCarrierListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public string SeventeenTrackCarrierAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<SeventeenTrackCarrierInfo> SeventeenTrackCarrierList { get; set; } = new List<SeventeenTrackCarrierInfo>();
}
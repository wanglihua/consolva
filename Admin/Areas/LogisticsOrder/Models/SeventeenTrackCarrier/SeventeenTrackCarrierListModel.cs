using shunda.Core;
using shunda.Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.LogisticsOrder.Models.SeventeenTrackCarrier;

public class SeventeenTrackCarrierListModel : ListModel {
    public SeventeenTrackCarrierListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public string SeventeenTrackCarrierAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<SeventeenTrackCarrierInfo> SeventeenTrackCarrierList { get; set; } = new List<SeventeenTrackCarrierInfo>();
}
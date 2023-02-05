using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetailOrderPack;

public class LogisticsOrderDetailOrderPackListViewCompenentModel {
    [BindNever]
    public IList<LogisticsOrderFirstModeInfo> OrderFirstModeList { get; set; } = new List<LogisticsOrderFirstModeInfo>();

    [BindNever]
    public LogisticsOrderPackDataInfo OrderPackData { get; set; } = new();

    [BindNever]
    public bool HasModifyOrderPackDataPass { get; set; }
}
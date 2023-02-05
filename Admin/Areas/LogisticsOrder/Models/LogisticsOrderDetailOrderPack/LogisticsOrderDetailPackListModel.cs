using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetailOrderPack;

public class LogisticsOrderDetailPackListModel {
    [BindNever]
    public IList<LogisticsOrderFirstModeInfo> OrderFirstModeList { get; set; } = new List<LogisticsOrderFirstModeInfo>();

    public LogisticsOrderPackDataInfo OrderPackData { get; set; } = new();

    public bool HasModifyOrderPackDataPass { get; set; }    
}
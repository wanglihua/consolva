using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderRemark;

public class LogisticsOrderRemarkOrderRemarkListModel {
    [BindNever]
    public int OrderId { get; set; }

    [BindNever]
    public IList<LogisticsOrderRemarkInfo> LogisticsOrderRemarkList { get; set; } = new List<LogisticsOrderRemarkInfo>();
}
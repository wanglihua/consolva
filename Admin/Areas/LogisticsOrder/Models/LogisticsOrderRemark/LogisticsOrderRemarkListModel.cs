using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderRemark;

public class LogisticsOrderRemarkListModel {
    [BindNever]
    public int OrderId { get; set; }

    [BindNever]
    public IList<LogisticsOrderRemarkInfo> OrderRemarkList { get; set; } = new List<LogisticsOrderRemarkInfo>();

    [BindNever]
    public IList<ExpressRemarkInfo> ExpressRemarkList { get; set; } = new List<ExpressRemarkInfo>();

    [BindNever]
    public bool CanOrderRemarkEdit { get; set; }
}
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderRemark;

public class LogisticsOrderRemarkListModel {
    [BindNever]
    public int OrderId { get; set; }
}
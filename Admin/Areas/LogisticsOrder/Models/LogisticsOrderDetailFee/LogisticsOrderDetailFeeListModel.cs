using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetailFee;

public class LogisticsOrderDetailFeeListModel {
    [BindNever]
    public int OrderId { get; set; }
}
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetailFee;

public class LogisticsOrderDetailFeeListModel {
    [BindNever]
    public LogisticsOrderInfo Order { get; set; } = new();

    [BindNever]
    public IList<LogisticsOrderFirstModeInfo> OrderFirstModeList { get; set; } = new List<LogisticsOrderFirstModeInfo>();

    [BindNever]
    public IList<LogisticsOrderFeeInfo> OrderFeeList { get; set; } = new List<LogisticsOrderFeeInfo>();
}
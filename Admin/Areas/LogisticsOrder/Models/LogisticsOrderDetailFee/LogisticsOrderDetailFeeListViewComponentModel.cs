using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetailFee;

public class LogisticsOrderDetailFeeListViewComponentModel {
    [BindNever]
    public LogisticsOrderInfo Order { get; set; } = new();

    /// <summary>
    /// 这里同时包含有订单打包信息
    /// </summary>
    [BindNever]
    public IList<LogisticsOrderFirstModeInfo> OrderFirstModeList { get; set; } = new List<LogisticsOrderFirstModeInfo>();

    [BindNever]
    public IList<LogisticsOrderFeeInfo> OrderFeeList { get; set; } = new List<LogisticsOrderFeeInfo>();
}
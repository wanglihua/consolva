using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderList;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetail;

public class LogisticsOrderDetailIndexModel {
    [BindNever]
    public OrderListMenuType MenuType { get; set; }

    [BindNever]
    public LogisticsOrderInfo LogisticsOrder { get; set; } = new();

    [BindNever]
    public IList<LogisticsOrderRemarkInfo> LogisticsOrderRemarkList { get; set; } = new List<LogisticsOrderRemarkInfo>();

    [BindNever]
    public LogisticsOrderReceiveAddressInfo ReceiveAddress { get; set; } = new();

    [BindNever]
    public LogisticsOrderSendAddressInfo SendAddress { get; set; } = new();
}
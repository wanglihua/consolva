using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderList;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetail;

public class LogisticsOrderDetailIndexModel {
    [BindNever]
    public OrderListMenuType MenuType { get; set; }

    [BindNever]
    public LogisticsOrderInfo LogisticsOrder { get; set; } = new();

    [BindNever]
    public IList<LogisticsOrderRemarkInfo> OrderRemarkList { get; set; } = new List<LogisticsOrderRemarkInfo>();

    [BindNever]
    public IList<ExpressRemarkInfo> ExpressRemarkList { get; set; } = new List<ExpressRemarkInfo>();

    [BindNever]
    public LogisticsOrderReceiveAddressInfo ReceiveAddress { get; set; } = new();

    [BindNever]
    public LogisticsOrderSendAddressInfo SendAddress { get; set; } = new();

    [BindNever]
    public IList<ExpressInfo> ExpressList { get; set; } = new List<ExpressInfo>();

    /// <summary>
    /// 这里同时包含有订单打包信息
    /// </summary>
    [BindNever]
    public IList<LogisticsOrderFirstModeInfo> OrderFirstModeList { get; set; } = new List<LogisticsOrderFirstModeInfo>();

    [BindNever]
    public IList<LogisticsOrderPackBoxInfo> OrderPackBoxList { get; set; } = new List<LogisticsOrderPackBoxInfo>();

    [BindNever]
    public IList<LogisticsOrderActionInfo> LogisticsOrderActionList { get; set; } = new List<LogisticsOrderActionInfo>();

    [BindNever]
    public bool CanOrderRemarkEdit { get; set; }
}
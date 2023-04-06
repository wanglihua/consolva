using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderList;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetail;

public class LogisticsOrderDetailIndexModel {
    [BindNever]
    public OrderListMenuType MenuType { get; set; }

    [BindNever]
    public LogisticsOrderInfo Order { get; set; } = new();

    [BindNever]
    public string RepackTypeTitle { get; set; } = string.Empty;

    [BindNever]
    public LogisticsOrderReceiveAddressInfo ReceiveAddress { get; set; } = new();

    // [BindNever]
    // public LogisticsOrderSendAddressInfo SendAddress { get; set; } = new();

    [BindNever]
    public IList<ExpressInfo> ExpressList { get; set; } = new List<ExpressInfo>();

    [BindNever]
    public IList<LogisticsOrderPackBoxInfo> OrderPackBoxList { get; set; } = new List<LogisticsOrderPackBoxInfo>();

    [BindNever]
    public IList<ExpressPhotoInfo> PhotoList { get; set; } = new List<ExpressPhotoInfo>();

    [BindNever]
    public IList<LogisticsOrderActionInfo> LogisticsOrderActionList { get; set; } = new List<LogisticsOrderActionInfo>();
}
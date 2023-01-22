using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderList;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetail; 

public class LogisticsOrderDetailIndexModel {
    [BindNever]
    public OrderListMenuType MenuType { get; set; }
}
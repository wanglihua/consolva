using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderList;

public class LogisticsOrderListModel : ListModel {
    public LogisticsOrderListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public IList<LogisticsOrderInfo> OrderList { get; set; } = new List<LogisticsOrderInfo>();
}
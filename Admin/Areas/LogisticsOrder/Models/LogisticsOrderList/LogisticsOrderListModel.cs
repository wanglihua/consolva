using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderList;

public class LogisticsOrderListModel : ListModel {
    public LogisticsOrderListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public IList<LogisticsOrderInfo> LogisticsOrderList { get; set; } = new List<LogisticsOrderInfo>();
}
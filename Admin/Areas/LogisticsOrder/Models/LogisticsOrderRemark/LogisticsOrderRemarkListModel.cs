using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderRemark;

public class LogisticsOrderRemarkListModel : ListModel {
    public LogisticsOrderRemarkListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public IList<LogisticsOrderRemarkInfo> LogisticsOrderRemarkList { get; set; } = new List<LogisticsOrderRemarkInfo>();
}
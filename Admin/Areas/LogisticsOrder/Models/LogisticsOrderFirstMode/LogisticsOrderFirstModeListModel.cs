using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderFirstMode;

public class LogisticsOrderFirstModeListModel : ListModel {
    public LogisticsOrderFirstModeListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public string LogisticsOrderFirstModeAlias { get; set; } = string.Empty;

    [BindNever]
    public string OrderAlias { get; set; } = string.Empty;

    [BindNever]
    public string FirstModeAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<LogisticsOrderFirstModeInfo> LogisticsOrderFirstModeList { get; set; } = new List<LogisticsOrderFirstModeInfo>();
}
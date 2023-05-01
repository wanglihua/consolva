using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderFirstMode;

public class LogisticsOrderFirstModeListModel : ListModel {
    public LogisticsOrderFirstModeListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public string OrderFirstModeAlias { get; set; } = string.Empty;

    [BindNever]
    public string OrderAlias { get; set; } = string.Empty;

    [BindNever]
    public string FirstModeAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<LogisticsOrderFirstModeViewInfo> LogisticsOrderFirstModeList { get; set; } = new List<LogisticsOrderFirstModeViewInfo>();
}
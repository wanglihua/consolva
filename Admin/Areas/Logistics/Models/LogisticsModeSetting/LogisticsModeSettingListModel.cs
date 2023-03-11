using shunda.Core;
using shunda.Domain.Logistics;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsModeSetting;

public class LogisticsModeSettingListModel : ListModel {
    public LogisticsModeSettingListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public string LogisticsModeSettingAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<LogisticsModeSettingInfo> LogisticsModeSettingList { get; set; } = new List<LogisticsModeSettingInfo>();
}
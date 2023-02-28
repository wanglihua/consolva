using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsRepackTypeSetting;

public class LogisticsRepackTypeSettingListModel {
    [BindNever]
    public string LogisticsRepackTypeSettingAlias { get; set; } = string.Empty;

    [BindNever]
    public IList<LogisticsRepackTypeSettingInfo> LogisticsRepackTypeSettingList { get; set; } = new List<LogisticsRepackTypeSettingInfo>();
}
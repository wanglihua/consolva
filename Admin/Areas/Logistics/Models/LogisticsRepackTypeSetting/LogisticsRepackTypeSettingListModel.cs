using shunda.Core;
using shunda.Domain.Logistics;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsRepackTypeSetting;

public class LogisticsRepackTypeSettingListModel : ListModel {
    public LogisticsRepackTypeSettingListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public string LogisticsRepackTypeSettingAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<LogisticsRepackTypeSettingInfo> LogisticsRepackTypeSettingList { get; set; } = new List<LogisticsRepackTypeSettingInfo>();
}
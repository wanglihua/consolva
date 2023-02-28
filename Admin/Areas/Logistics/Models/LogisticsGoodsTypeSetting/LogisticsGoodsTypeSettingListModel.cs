using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsTypeSetting;

public class LogisticsGoodsTypeSettingListModel {
    [BindNever]
    public string LogisticsGoodsTypeSettingAlias { get; set; } = string.Empty;

    [BindNever]
    public IList<LogisticsGoodsTypeSettingInfo> LogisticsGoodsTypeSettingList { get; set; } = new List<LogisticsGoodsTypeSettingInfo>();
}
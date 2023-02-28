using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsRepackTypeSetting;

public class LogisticsRepackTypeSettingIndexModel {
    [BindNever]
    public IList<LogisticsRepackType> TypeList { get; set; } = new List<LogisticsRepackType>();

    [BindNever]
    public IList<LogisticsRepackTypeStatus> StatusList { get; set; } = new List<LogisticsRepackTypeStatus>();
}
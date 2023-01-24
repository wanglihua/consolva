using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderPackConfig;

public class LogisticsOrderPackConfigIndexModel : LogisticsOrderPackConfigSaveModel {
    [BindNever]
    public IList<LogisticsOrderPackSortType> SortTypeList { get; set; } = LogisticsOrderPackSortType.GetList();

    [BindNever]
    public IList<LogisticsOrderPackDisplayType> DisplayTypeList { get; set; } = LogisticsOrderPackDisplayType.GetList();
}
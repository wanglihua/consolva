using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderFirstMode;

public class LogisticsOrderFirstModeDetailModel : LogisticsOrderFirstModeSaveModel {
    [BindNever]
    public string OrderCode { get; set; } = string.Empty;

    [BindNever]
    public string FirstModeName { get; set; } = string.Empty;

    [BindNever]
    public LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;
}
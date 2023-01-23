using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderRemark;

public class LogisticsOrderRemarkDetailModel : LogisticsOrderRemarkSaveModel {
    [BindNever]
    public string OrderCode { get; set; } = string.Empty;
}
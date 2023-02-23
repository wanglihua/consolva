using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsFeeRateExpressEntry;

public class LogisticsFeeRateExpressEntryIndexModel {
    [BindNever]
    public LogisticsModeInfo LogisticsMode { get; set; } = new();

    [BindNever]
    public LogisticsFeeRateExpressInfo FeeRateExpress { get; set; } = new();
}
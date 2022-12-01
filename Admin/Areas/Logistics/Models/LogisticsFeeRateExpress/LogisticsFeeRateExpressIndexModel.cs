using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsFeeRateExpress;

public class LogisticsFeeRateExpressIndexModel : LogisticsFeeRateExpressSaveModel
{
    [BindNever]
    public int LogisticsModeId { get; set; }

    [BindNever]
    public LogisticsModeInfo LogisticsMode { get; set; } = new();
}
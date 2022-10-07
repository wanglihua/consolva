using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsFeeRateExpress;

public class LogisticsFeeRateExpressIndexModel
{
    [BindNever]
    public int LogisticsModeId { get; set; }

    [BindNever]
    public LogisticsModeInfo LogisticsMode { get; set; } = new LogisticsModeInfo();

    public LogisticsFeeRateExpressInfo LogisticsFeeRateExpress { get; set; } = new LogisticsFeeRateExpressInfo();
}
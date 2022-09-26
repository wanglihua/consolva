using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Models.LogisticsFeeRate;

public class LogisticsFeeRateIndexModel
{
    [BindNever]
    public IList<LogisticsFeeRateStatus> StatusList { get; set; } = new List<LogisticsFeeRateStatus>();
}
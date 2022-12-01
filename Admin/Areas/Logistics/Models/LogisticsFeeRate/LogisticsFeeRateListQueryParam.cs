using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsFeeRate;

public class LogisticsFeeRateListQueryParam : ListQueryParam
{
    public string? NameQuery { get; set; }

    [ModelBinder(typeof(LogisticsFeeRateStatusModelBinder))]
    public LogisticsFeeRateStatus? StatusQuery { get; set; }
}
using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsModeInner;

public class LogisticsModeInnerListQueryParam : ListQueryParam
{
    public int? TypeIdQuery { get; set; }

    [ModelBinder(typeof(LogisticsModeStatusModelBinder))]
    public LogisticsModeStatus? StatusQuery { get; set; }

    public string? CodeQuery { get; set; }

    public string? NameQuery { get; set; }
}
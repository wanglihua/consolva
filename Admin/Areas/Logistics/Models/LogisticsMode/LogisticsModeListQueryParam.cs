using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsMode;

public class LogisticsModeListQueryParam : ListQueryParam {
    // 这个不能为空，必须要有一个值
    public Country Country { get; set; } = Country.None;

    public LogisticsModeType? TypeQuery { get; set; }

    [ModelBinder(typeof(LogisticsModeStatusModelBinder))]
    public LogisticsModeStatus? StatusQuery { get; set; }

    public string? CodeQuery { get; set; }

    public string? NameQuery { get; set; }
}
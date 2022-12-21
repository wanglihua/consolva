using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsModeInner;

public class LogisticsModeInnerListQueryParam : ListQueryParam {
    // 这个不能为空，必须要有一个值
    [ModelBinder(typeof(CountryModelBinder))]
    public Country Country { get; set; } = Country.None;

    [ModelBinder(typeof(LogisticsModeTypeModelBinder))]
    public LogisticsModeType? TypeQuery { get; set; }

    [ModelBinder(typeof(LogisticsModeStatusModelBinder))]
    public LogisticsModeStatus? StatusQuery { get; set; }

    public string? CodeQuery { get; set; }

    public string? NameQuery { get; set; }
}
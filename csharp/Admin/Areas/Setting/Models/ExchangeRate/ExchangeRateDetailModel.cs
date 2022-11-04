using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain;

namespace shunda.Admin.Areas.Setting.Models.ExchangeRate;

public class ExchangeRateDetailModel : ExchangeRateSaveModel
{
    [BindNever]
    public ExchangeRateInfo ExchangeRate { get; set; } = new ExchangeRateInfo();
}
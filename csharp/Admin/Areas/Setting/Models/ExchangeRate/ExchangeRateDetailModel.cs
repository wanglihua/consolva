using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Setting.Models.ExchangeRate;

public class ExchangeRateDetailModel : ExchangeRateSaveModel
{
    [BindNever]
    public ExchangeRateInfo ExchangeRate { get; set; } = new ExchangeRateInfo();
}
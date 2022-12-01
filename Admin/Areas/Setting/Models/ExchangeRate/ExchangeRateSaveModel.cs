using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.Setting.Models.ExchangeRate;

public class ExchangeRateSaveModel
{
    [Required(ErrorMessage = "汇率Id不能为空！")]
    public int ExchangeRateId { get; set; }

    [Required(ErrorMessage = "汇率修正不能为空！")]
    public decimal RateFix { get; set; }
}
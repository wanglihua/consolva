using shunda.Core;

namespace shunda.Admin.Areas.Fee.Models.ManualDecreaseFee;

public class ManualDecreaseFeeListQueryParam : ListQueryParam
{
    public int ManualReasonIdQuery { get; set; }

    public string? OrderCodeQuery { get; set; }

    public string? CustomerCodeQuery { get; set; }

    public DateTime? BeginDateQuery { get; set; }

    public DateTime? EndDateQuery { get; set; }
}
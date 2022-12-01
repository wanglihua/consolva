using shunda.Core;

namespace shunda.Admin.Areas.Fee.Models.ManualIncreaseFee;

public class ManualIncreaseFeeListQueryParam : ListQueryParam
{
    public int ManualReasonIdQuery { get; set; }

    public string? OrderCodeQuery { get; set; }

    public string? CustomerCodeQuery { get; set; }
}
using shunda.Core;

namespace shunda.Admin.Areas.Fee.Models.WriteOffFee;

public class WriteOffFeeListQueryParam : ListQueryParam
{
    public int ManualReasonIdQuery { get; set; }

    public string? OrderCodeQuery { get; set; }

    public string? CustomerCodeQuery { get; set; }

    public DateTime? BeginDateQuery { get; set; }

    public DateTime? EndDateQuery { get; set; }
}
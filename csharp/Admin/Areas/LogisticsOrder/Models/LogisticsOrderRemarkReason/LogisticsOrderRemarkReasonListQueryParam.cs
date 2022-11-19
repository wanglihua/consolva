using shunda.Core;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderRemarkReason;

public class LogisticsOrderRemarkReasonListQueryParam : ListQueryParam
{
    public string? ReasonQuery { get; set; }

    public string? DescriptionQuery { get; set; }
}

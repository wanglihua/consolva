using shunda.Core;

namespace shunda.Admin.Areas.OrderFee.Models.LogisticsOrderPay;

public class LogisticsOrderPayListQueryParam : ListQueryParam
{
    public string? CustomerCodeQuery { get; set; }

    public string? OrderCodeQuery { get; set; }

    public int? LogisticsModeTypeIdQuery { get; set; }

    public DateTime? PayTimeBeginQuery { get; set; }

    public DateTime? PayTimeEndQuery { get; set; }
}
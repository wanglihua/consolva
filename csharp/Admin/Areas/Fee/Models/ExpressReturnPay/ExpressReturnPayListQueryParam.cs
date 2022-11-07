using shunda.Core;

namespace shunda.Admin.Areas.Fee.Models.ExpressReturnPay;

public class ExpressReturnPayListQueryParam : ListQueryParam
{
    public string? CustomerCodeQuery { get; set; }

    public DateTime? PayTimeBeginQuery { get; set; }

    public DateTime? PayTimeEndQuery { get; set; }
}
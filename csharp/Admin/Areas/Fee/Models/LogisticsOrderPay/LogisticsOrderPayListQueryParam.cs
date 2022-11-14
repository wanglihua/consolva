using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;

namespace shunda.Admin.Areas.Fee.Models.LogisticsOrderPay;

public class LogisticsOrderPayListQueryParam : ListQueryParam
{
    public string? CustomerCodeQuery { get; set; }

    public string? OrderCodeQuery { get; set; }

    public int? LogisticsModeTypeIdQuery { get; set; }

    public DateTime? PayTimeBeginQuery { get; set; }

    public DateTime? PayTimeEndQuery { get; set; }

    /// <summary>
    /// 用来存放 从 OrderCodeQuery 获得的 order id
    /// </summary>
    [BindNever]
    public int OrderId { get; set; }
}
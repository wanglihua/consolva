using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;

namespace shunda.Admin.Areas.Customer.Models.LogisticsOrderPayCustomer;

public class LogisticsOrderPayCustomerListQueryParam : ListQueryParam
{
    public int CustomerId { get; set; }

    public string? OrderCodeQuery { get; set; }

    /// <summary>
    /// 用来存放 从 OrderCodeQuery 获得的 order id
    /// </summary>
    [BindNever]
    public int OrderId { get; set; }
}
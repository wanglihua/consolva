using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Coupon;

namespace shunda.Admin.Areas.Fee.Models.Coupon;

public class CouponListQueryParam : ListQueryParam
{
    public int BatchId { get; set; }

    public string? CodeQuery { get; set; }

    public string? CustomerCodeQuery { get; set; }

    public string? OrderCodeQuery { get; set; }

    [BindNever]
    public int? OrderId { get; set; } = null;

    [ModelBinder(typeof(CouponStatusModelBinder))]
    public CouponStatus? StatusQuery { get; set; }
}
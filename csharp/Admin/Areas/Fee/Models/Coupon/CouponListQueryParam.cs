using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Coupon;

namespace shunda.Admin.Areas.Fee.Models.Coupon;

public class CouponListQueryParam : ListQueryParam
{
    public int BatchId { get; set; }

    public string? CodeQuery { get; set; }

    public string? CustomerCodeQuery { get; set; }

    public string? OrderCodeQuery { get; set; }

    [ModelBinder(typeof(CouponStatusModelBinder))]
    public CouponStatus? StatusQuery { get; set; }
}
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Coupon;

namespace shunda.Admin.Areas.Fee.Models.Coupon;

public class CouponIndexModel
{
    [BindNever]
    public CouponBatchInfo Batch { get; set; } = new();

    [BindNever]
    public IList<CouponStatus> StatusList { get; set; } = CouponStatus.GetList();
}
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Coupon;

namespace shunda.Admin.Areas.Fee.Models.CouponBatchFollow;

public class CouponBatchFollowIndexModel {
    [BindNever]
    public IList<CouponType> TypeList { get; set; } = CouponType.GetList();

    [BindNever]
    public IList<CouponBatchStatus> StatusList { get; set; } = CouponBatchStatus.GetList();
}
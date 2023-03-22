using shunda.Core;
using shunda.Domain.Coupon;

namespace shunda.Admin.Areas.Fee.Models.CouponBatchFollow;

public class CouponBatchFollowListModel : ListModel {
    public CouponBatchFollowListModel(ListPager listPager) : base(listPager) {
    }

    public IList<CouponBatchInfo> CouponBatchList { get; set; } = new List<CouponBatchInfo>();
}
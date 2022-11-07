using shunda.Core;
using shunda.Domain.Coupon;

namespace shunda.Admin.Areas.Fee.Models.Coupon;

public class CouponListModel : ListModel
{
    public CouponListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<CouponInfo> CouponList { get; set; } = new List<CouponInfo>();
}
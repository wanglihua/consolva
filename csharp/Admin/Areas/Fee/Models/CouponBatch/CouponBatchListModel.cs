using shunda.Core;
using shunda.Domain.Coupon;

namespace shunda.Admin.Areas.Fee.Models.CouponBatch;

public class CouponBatchListModel : ListModel
{
    public CouponBatchListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<CouponBatchInfo> CouponBatchList { get; set; } = new List<CouponBatchInfo>();
}
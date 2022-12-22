using Newtonsoft.Json;

namespace shunda.Client.Areas.Coupon.Models.CouponOrder;

public class CouponOrderListModel {
    [JsonProperty("couponList")]
    public IList<CouponOrderListItem> CouponList { get; set; } = new List<CouponOrderListItem>();

    /// <summary>
    /// 当前选择的优惠券
    /// 可能是上一次创建订单时选择了优惠券，留下的选择记录，但没有最后提交创建订单
    /// </summary>
    [JsonProperty("couponId")]
    public int CouponId { get; set; }
}
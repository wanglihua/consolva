using Newtonsoft.Json;

namespace shunda.Client.Areas.Coupon.Models.CouponPay;

public class CouponPayListModel {
    [JsonProperty("couponList")]
    public IList<CouponPayListItem> CouponList { get; set; } = new List<CouponPayListItem>();

    [JsonProperty("couponId")]
    public int CouponId { get; set; }
}
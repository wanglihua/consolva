using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderPayCoupon;

public class OrderPayCouponListModel {
    [JsonProperty("couponCount")]
    public int CouponCount { get; set; }

    [JsonProperty("couponList")]
    public IList<OrderPayCouponListItem> CouponList { get; set; } = new List<OrderPayCouponListItem>();

    [JsonProperty("couponId")]
    public int CouponId { get; set; }
}
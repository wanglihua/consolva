using Newtonsoft.Json;

namespace shunda.Client.Areas.Coupon.Models.CouponOrder;

public class CouponOrderSaveParam {
    [JsonProperty("couponId")]
    public int CouponId { get; set; }
}
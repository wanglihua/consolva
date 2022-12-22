using Newtonsoft.Json;

namespace shunda.Client.Areas.Coupon.Models.CouponPay; 

public class CouponPaySaveParam {
    [JsonProperty("couponId")]
    public int CouponId { get; set; }
}
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Coupon.Models.CouponPay;

public class CouponPaySaveParam {
    [Required(ErrorMessage = "订单Id不能为空！")]
    [JsonProperty("orderId")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "优惠券Id不能为空！")]
    [JsonProperty("couponId")]
    public int CouponId { get; set; }
}
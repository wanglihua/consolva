using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Coupon.Models.CouponPay;

public class CouponPayListParam {
    [Required(ErrorMessage = "订单Id不能为空！")]
    [JsonProperty("orderId")]
    public int OrderId { get; set; }
}
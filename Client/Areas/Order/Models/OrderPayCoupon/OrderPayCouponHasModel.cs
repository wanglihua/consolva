using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderPayCoupon;

public class OrderPayCouponHasModel {
    [JsonProperty("hasCoupon")]
    public bool HasCoupon { get; set; }
}
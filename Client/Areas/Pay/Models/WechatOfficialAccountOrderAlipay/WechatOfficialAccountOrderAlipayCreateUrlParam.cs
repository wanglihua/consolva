using Newtonsoft.Json;

namespace shunda.Client.Areas.Pay.Models.WechatOfficialAccountOrderAlipay;

public class WechatOfficialAccountOrderAlipayCreateUrlParam {
    [JsonProperty("orderId")]
    public int OrderId { get; set; }

    [JsonProperty("couponId")]
    public int CouponId { get; set; }
}
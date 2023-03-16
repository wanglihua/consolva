using Newtonsoft.Json;

namespace shunda.Client.Areas.Pay.Models.WechatOfficialAccountChargeAlipay; 

public class WechatOfficialAccountChargeAlipayCreateUrlParam {
    [JsonProperty("customerId")]
    public int CustomerId { get; set; }

    [JsonProperty("amount")]
    public decimal Amount { get; set; }
}
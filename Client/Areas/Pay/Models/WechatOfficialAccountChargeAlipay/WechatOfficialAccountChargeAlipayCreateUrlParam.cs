using Newtonsoft.Json;

namespace shunda.Client.Areas.Pay.Models.WechatOfficialAccountChargeAlipay; 

public class WechatOfficialAccountChargeAlipayCreateUrlParam {
    [JsonProperty("amount")]
    public decimal Amount { get; set; }
}

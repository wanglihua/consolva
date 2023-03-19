using Newtonsoft.Json;

namespace shunda.Client.Areas.Pay.Models.WechatOfficialAccountChargePaypal; 

public class WechatOfficialAccountChargePaypalCreateUrlParam {
    [JsonProperty("amount")]
    public decimal Amount { get; set; }
}
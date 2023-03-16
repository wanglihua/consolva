using Newtonsoft.Json;

namespace shunda.Client.Areas.Pay.Models.WechatOfficialAccountOrderAlipay;

public class WechatOfficialAccountOrderAlipayCreateUrlModel {
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}
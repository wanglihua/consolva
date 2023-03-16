using Newtonsoft.Json;

namespace shunda.Client.Areas.Pay.Models.WechatOfficialAccountChargeAlipay;

public class WechatOfficialAccountChargeAlipayCreateUrlModel {
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}
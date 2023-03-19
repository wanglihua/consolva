using Newtonsoft.Json;

namespace shunda.Client.Areas.Pay.Models.WechatOfficialAccountChargePaypal;

public class WechatOfficialAccountChargePaypalCreateUrlModel {
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}
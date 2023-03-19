using Newtonsoft.Json;

namespace shunda.Client.Areas.Pay.Models.WechatOfficialAccountOrderPaypal;

public class WechatOfficialAccountOrderPaypalCreateUrlModel {
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}
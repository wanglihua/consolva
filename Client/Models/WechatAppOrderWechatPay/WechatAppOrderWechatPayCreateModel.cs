using Newtonsoft.Json;

namespace shunda.Client.Models.WechatAppOrderWechatPay;

public class WechatAppOrderPayCreatePayModel {
    [JsonProperty("timestamp")]
    public string TimeStamp { get; set; } = string.Empty;

    [JsonProperty("noncestr")]
    public string NonceStr { get; set; } = string.Empty;

    [JsonProperty("package")]
    public string Package { get; set; } = string.Empty;

    [JsonProperty("signtype")]
    public string SignType { get; set; } = string.Empty;

    [JsonProperty("paysign")]
    public string PaySign { get; set; } = string.Empty;
}
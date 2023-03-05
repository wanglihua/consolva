using Newtonsoft.Json;

namespace shunda.Service.Pay;

public class WechatPayCreateResultData {
    [JsonProperty("needPay")]
    public bool NeedPay { get; set; } = true;

    [JsonProperty("timeStamp")]
    public string TimeStamp { get; set; } = string.Empty;

    [JsonProperty("nonceStr")]
    public string NonceStr { get; set; } = string.Empty;

    [JsonProperty("package")]
    public string Package { get; set; } = string.Empty;

    [JsonProperty("signType")]
    public string SignType { get; set; } = string.Empty;

    [JsonProperty("paySign")]
    public string PaySign { get; set; } = string.Empty;
}
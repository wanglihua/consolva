using Newtonsoft.Json;

namespace shunda.Client.Models.WechatAppOrderWechatPay;

public class WechatAppOrderWechatPayCreateParam {
    [JsonProperty("orderId")]
    public int OrderId { get; set; }
}


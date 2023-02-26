using Newtonsoft.Json;

namespace shunda.Client.Models.WechatAppOrderPay;

public class WechatAppOrderPayCreatePayParam {
    [JsonProperty("orderId")]
    public int OrderId { get; set; }
}


using Newtonsoft.Json;

namespace shunda.Client.Areas.Pay.Models.WechatOfficialAccountChargePaypal; 

public class WechatOfficialAccountChargePaypalCreateUrlParam {
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 货币代码 AUD，USD
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; } = string.Empty;
}
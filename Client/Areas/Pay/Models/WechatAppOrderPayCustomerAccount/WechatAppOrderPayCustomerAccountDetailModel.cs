using Newtonsoft.Json;

namespace shunda.Client.Areas.Pay.Models.WechatAppOrderPayCustomerAccount;

public class WechatAppOrderPayCustomerAccountDetailModel {
    [JsonProperty("orderId")]
    public int OrderId { get; set; }

    [JsonProperty("orderCode")]
    public string OrderCode { get; set; } = string.Empty;

    [JsonProperty("totalAmount")]
    public string TotalAmount { get; set; } = string.Empty;

    [JsonProperty("hasCoupon")]
    public bool HasCoupon { get; set; }

    [JsonProperty("discountAmount")]
    public string DiscountAmount { get; set; } = string.Empty;

    [JsonProperty("paidAmount")]
    public string PaidAmount { get; set; } = string.Empty;

    [JsonProperty("arrearsAmount")]
    public string ArrearsAmount { get; set; } = string.Empty;

    [JsonProperty("actualPayAmount")]
    public string ActualPayAmount { get; set; } = string.Empty;

    [JsonProperty("hasAccountToPay")]
    public bool HasAccountToPay { get; set; }
}
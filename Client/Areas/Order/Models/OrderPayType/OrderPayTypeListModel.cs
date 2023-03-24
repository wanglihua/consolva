using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderPayType;

public class OrderPayTypeListModel {
    [JsonProperty("orderId")]
    public int OrderId { get; set; }

    [JsonProperty("orderCode")]
    public string OrderCode { get; set; } = string.Empty;

    [JsonProperty("totalAmount")]
    public string TotalAmount { get; set; } = string.Empty;

    [JsonProperty("vipDiscountAmount")]
    public string VipDiscountAmount { get; set; } = string.Empty;

    [JsonProperty("customerScore")]
    public string CustomerScore { get; set; } = string.Empty;

    [JsonProperty("scoreDeductScore")]
    public string ScoreDeductScore { get; set; } = string.Empty;

    [JsonProperty("scoreDeductAmount")]
    public string ScoreDeductAmount { get; set; } = string.Empty;

    [JsonProperty("hasCoupon")]
    public bool HasCoupon { get; set; }

    [JsonProperty("couponDiscountAmount")]
    public string CouponDiscountAmount { get; set; } = string.Empty;

    [JsonProperty("paidAmount")]
    public string PaidAmount { get; set; } = string.Empty;

    [JsonProperty("arrearsAmount")]
    public string ArrearsAmount { get; set; } = string.Empty;
}
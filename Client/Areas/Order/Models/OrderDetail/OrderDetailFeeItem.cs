using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailFeeItem {
    [JsonProperty("firstModeList")]
    public IList<OrderDetailFeeFirstModeItem> FirstModeList = new List<OrderDetailFeeFirstModeItem>();

    [JsonProperty("otherFeeList")]
    public IList<OrderDetailFeeListItem> OtherFeeList = new List<OrderDetailFeeListItem>();

    [JsonProperty("totalAmount")]
    public string TotalAmount { get; set; } = string.Empty;

    [JsonProperty("paidAmount")]
    public string PaidAmount { get; set; } = string.Empty;

    [JsonProperty("arrearsAmount")]
    public string ArrearsAmount { get; set; } = string.Empty;
}
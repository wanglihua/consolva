using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailFeeItem {
    [JsonProperty("firstModeList")]
    public IList<OrderDetailFeeFirstModeItem> FirstModeList = new List<OrderDetailFeeFirstModeItem>();

    [JsonProperty("otherFeeList")]
    public IList<OrderDetailFeeListItem> OtherFeeList = new List<OrderDetailFeeListItem>();

    [JsonProperty("totalAmount")]
    public string TotalAmount { get; set; } = string.Empty;

    [JsonProperty("arearsAmount")]
    public string ArearsAmount { get; set; } = string.Empty;
}
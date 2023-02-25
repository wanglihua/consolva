using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailFeeListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("feeType")]
    public string FeeType { get; set; } = string.Empty;

    [JsonProperty("amount")]
    public string Amount { get; set; } = string.Empty;
}
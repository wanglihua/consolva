using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailFirstModeItem {
    [JsonProperty("firstModeList")]
    public IList<OrderDetailFirstModeListItem> FirstModeList { get; set; } = new List<OrderDetailFirstModeListItem>();
}
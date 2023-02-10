using Newtonsoft.Json;
using shunda.Service.LogisticsOrder;

namespace shunda.Client.Areas.Order.Models.OrderList;

public class OrderListModel {
    [JsonProperty("orderList")]
    public IList<LogisticsOrderClientListItem> OrderList { get; set; } = new List<LogisticsOrderClientListItem>();

    [JsonProperty("orderCount")]
    public int OrderCount { get; set; }

    [JsonProperty("lastId")]
    public int LastId { get; set; }
}
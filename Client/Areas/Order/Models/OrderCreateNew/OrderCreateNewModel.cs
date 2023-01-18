using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderCreateNew;

public class OrderCreateNewModel {
    [JsonProperty("orderId")]
    public int OrderId { get; set; }
}
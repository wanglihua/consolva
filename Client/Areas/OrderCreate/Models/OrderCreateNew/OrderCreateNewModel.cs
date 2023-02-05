using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.OrderCreateNew;

public class OrderCreateNewModel {
    [JsonProperty("orderId")]
    public int OrderId { get; set; }
}
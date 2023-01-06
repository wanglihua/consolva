using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderCreateAddressReceive;

public class OrderCreateAddressReceiveListItem {
    [JsonProperty("id")]
    public int Id { get; set; }
}
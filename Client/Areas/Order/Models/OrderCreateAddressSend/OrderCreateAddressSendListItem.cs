using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderCreateAddressSend;

public class OrderCreateAddressSendListItem {
    [JsonProperty("id")]
    public int Id { get; set; }
}
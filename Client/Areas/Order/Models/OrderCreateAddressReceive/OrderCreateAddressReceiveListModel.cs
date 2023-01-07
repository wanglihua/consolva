using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderCreateAddressReceive;

public class OrderCreateAddressReceiveListModel {
    [JsonProperty("addressList")]
    public IList<OrderCreateAddressReceiveListItem> AddressList = new List<OrderCreateAddressReceiveListItem>();

    [JsonProperty("selectAddressId")]
    public int SelectAddressId { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; } = string.Empty;
}
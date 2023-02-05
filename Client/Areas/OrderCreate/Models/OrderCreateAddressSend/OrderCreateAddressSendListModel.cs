using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.OrderCreateAddressSend;

public class OrderCreateAddressSendListModel {
    [JsonProperty("addressList")]
    public IList<OrderCreateAddressSendListItem> AddressList = new List<OrderCreateAddressSendListItem>();

    [JsonProperty("selectAddressId")]
    public int SelectAddressId { get; set; }
}
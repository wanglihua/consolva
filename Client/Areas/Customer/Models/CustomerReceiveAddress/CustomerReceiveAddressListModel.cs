using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerReceiveAddress;

public class CustomerReceiveAddressListModel {
    [JsonProperty("receiveAddressList")]
    public IList<CustomerReceiveAddressListItem> ReceiveAddressList = new List<CustomerReceiveAddressListItem>();

    [JsonProperty("receiveAddressCount")]
    public int ReceiveAddressCount { get; set; }

    [JsonProperty("sendAddressCount")]
    public int SendAddressCount { get; set; }
}
using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerSendAddress;

public class CustomerSendAddressListModel {
    [JsonProperty("sendAddressList")]
    public IList<CustomerSendAddressListItem> SendAddressList = new List<CustomerSendAddressListItem>();

    [JsonProperty("sendAddressCount")]
    public int SendAddressCount { get; set; }

    [JsonProperty("receiveAddressCount")]
    public int ReceiveAddressCount { get; set; }
}
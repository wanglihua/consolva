using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerSendAddress;

public class CustomerSendAddressListModel
{
    [JsonProperty("addressList")]
    public IList<CustomerSendAddressListItem> AddressList = new List<CustomerSendAddressListItem>();

    [JsonProperty("addressCount")]
    public int AddressCount { get; set; }
}
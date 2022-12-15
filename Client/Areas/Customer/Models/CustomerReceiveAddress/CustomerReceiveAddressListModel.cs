using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerReceiveAddress;

public class CustomerReceiveAddressListModel
{
    [JsonProperty("addressList")]
    public IList<CustomerReceiveAddressListItem> AddressList = new List<CustomerReceiveAddressListItem>();

    [JsonProperty("addressCount")]
    public int AddressCount { get; set; }
}
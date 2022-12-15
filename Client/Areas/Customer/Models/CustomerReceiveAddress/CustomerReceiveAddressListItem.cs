using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerReceiveAddress;

public class CustomerReceiveAddressListItem
{
    [JsonProperty("id")]
    public int Id { get; set; }
}
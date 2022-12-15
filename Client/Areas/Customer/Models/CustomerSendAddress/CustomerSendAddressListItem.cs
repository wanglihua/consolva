using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerSendAddress;

public class CustomerSendAddressListItem
{
    [JsonProperty("id")]
    public int Id { get; set; }
}

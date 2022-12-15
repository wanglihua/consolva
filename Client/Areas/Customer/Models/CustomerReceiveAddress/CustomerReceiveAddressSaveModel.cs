using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerReceiveAddress;

public class CustomerReceiveAddressSaveModel
{
    [JsonProperty("id")]
    public int Id { get; set; }
}
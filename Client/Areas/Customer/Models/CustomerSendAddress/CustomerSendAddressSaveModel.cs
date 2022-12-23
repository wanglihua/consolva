using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerSendAddress;

public class CustomerSendAddressSaveModel {
    [JsonProperty("id")]
    public int Id { get; set; }
}
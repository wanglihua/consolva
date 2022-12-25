using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerSendAddress;

public class CustomerSendAddressRegionItem {
    [JsonProperty("value")]
    public string Value { get; set; } = string.Empty;

    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;

    [JsonProperty("children", NullValueHandling = NullValueHandling.Ignore)]
    public IList<CustomerSendAddressRegionItem>? Children { get; set; }
}
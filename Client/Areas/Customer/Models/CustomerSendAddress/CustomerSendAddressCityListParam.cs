using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerSendAddress;

public class CustomerSendAddressCityListParam {
    [JsonProperty("regionId")]
    public int RegionId { get; set; }

    [JsonProperty("tabIndex")]
    public int TabIndex { get; set; }
}
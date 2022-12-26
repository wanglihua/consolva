using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerSendAddress;

public class CustomerSendAddressDetailModel : CustomerSendAddressSaveModel {
    [JsonProperty("regionList")]
    public IList<CustomerSendAddressRegionItem> RegionList { get; set; } = new List<CustomerSendAddressRegionItem>();

    [JsonProperty("streetFieldValue")]
    public string StreetFieldValue { get; set; } = string.Empty;

    [JsonProperty("streetCascaderValue")]
    public string StreetCascaderValue { get; set; } = string.Empty;
}
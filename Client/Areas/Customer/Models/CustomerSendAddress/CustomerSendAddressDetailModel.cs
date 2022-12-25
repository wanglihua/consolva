using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerSendAddress;

public class CustomerSendAddressDetailModel : CustomerSendAddressSaveModel {
    [JsonProperty("provinceList")]
    public IList<CustomerSendAddressRegionItem> ProvinceList { get; set; } = new List<CustomerSendAddressRegionItem>();
}
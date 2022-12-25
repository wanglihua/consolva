using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerSendAddress; 

public class CustomerSendAddressCityListParam {
    [JsonProperty("provinceId")]
    public int ProvinceId { get; set; }
}
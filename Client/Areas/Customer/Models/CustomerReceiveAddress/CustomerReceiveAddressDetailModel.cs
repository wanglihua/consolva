using Newtonsoft.Json;
using shunda.Domain;

namespace shunda.Client.Areas.Customer.Models.CustomerReceiveAddress;

public class CustomerReceiveAddressDetailModel : CustomerReceiveAddressSaveModel {
    [JsonProperty("countryList")]
    public IList<Country> CountryList { get; set; } = Country.GetNoChinaList();
}
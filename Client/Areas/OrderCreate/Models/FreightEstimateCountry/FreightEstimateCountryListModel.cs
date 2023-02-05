using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.FreightEstimateCountry;

public class FreightEstimateCountryListModel {
    [JsonProperty("countryList")]
    public IList<FreightEstimateCountryListItem> CountryList = new List<FreightEstimateCountryListItem>();

    [JsonProperty("selectCountryId")]
    public int SelectCountryId { get; set; }
}
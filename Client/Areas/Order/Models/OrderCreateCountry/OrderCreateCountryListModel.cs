using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderCreateCountry;

public class OrderCreateCountryListModel {
    [JsonProperty("countryList")]
    public IList<OrderCreateCountryListItem> CountryList { get; set; } = new List<OrderCreateCountryListItem>();
}
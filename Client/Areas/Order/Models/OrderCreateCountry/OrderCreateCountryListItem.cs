using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderCreateCountry;

public class OrderCreateCountryListItem {
    [JsonProperty("countryId")]
    public int CountryId { get; set; }

    [JsonProperty("countryName")]
    public string CountryName { get; set; } = string.Empty;
}
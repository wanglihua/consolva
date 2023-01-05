using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderCreateCountry;

public class OrderCreateCountrySaveParam {
    [JsonProperty("countryId")]
    public int CountryId { get; set; }
}
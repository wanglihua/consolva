using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.FreightEstimateCountry;

public class FreightEstimateCountryListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}
using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.OrderCreateInsurance;

public class OrderCreateInsuranceListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}
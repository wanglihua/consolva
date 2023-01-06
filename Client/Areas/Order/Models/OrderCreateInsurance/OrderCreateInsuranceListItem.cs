using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderCreateInsurance; 

public class OrderCreateInsuranceListItem {
    [JsonProperty("id")]
    public int Id { get; set; }
}
using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.FreightEstimateAddressReceive; 

public class FreightEstimateAddressReceiveListItem {
    [JsonProperty("id")]
    public int Id { get; set; }
}
using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.FreightEstimateAddressReceive;

public class FreightEstimateAddressReceiveListModel {
    [JsonProperty("addressList")]
    public IList<FreightEstimateAddressReceiveListItem> AddressList = new List<FreightEstimateAddressReceiveListItem>();

    [JsonProperty("selectAddressId")]
    public int SelectAddressId { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; } = string.Empty;
}
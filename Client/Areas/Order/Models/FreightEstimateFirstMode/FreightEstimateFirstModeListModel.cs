using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.FreightEstimateFirstMode;

public class FreightEstimateFirstModeListModel {
    [JsonProperty("firstModeList")]
    public IList<FreightEstimateFirstModeListItem> FirstModeList = new List<FreightEstimateFirstModeListItem>();

    [JsonProperty("selectFirstModeId")]
    public int SelectFirstModeId { get; set; }
}
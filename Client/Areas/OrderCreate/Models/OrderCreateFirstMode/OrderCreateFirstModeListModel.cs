using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.OrderCreateFirstMode;

public class OrderCreateFirstModeListModel {
    [JsonProperty("firstModeList")]
    public IList<OrderCreateFirstModeListItem> FirstModeList = new List<OrderCreateFirstModeListItem>();

    [JsonProperty("selectFirstModeId")]
    public int SelectFirstModeId { get; set; }
}
using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderCreateFirstMode; 

public class OrderCreateFirstModeListModel {

    [JsonProperty("firstModeList")]
    public IList<OrderCreateFirstModeListItem> FirstModeList = new List<OrderCreateFirstModeListItem>();
}
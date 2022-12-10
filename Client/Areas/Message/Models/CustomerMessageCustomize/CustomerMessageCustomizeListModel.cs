using Newtonsoft.Json;

namespace shunda.Client.Areas.Message.Models.CustomerMessageCustomize;

public class CustomerMessageCustomizeListModel
{
    [JsonProperty("messageCustomizeList")]
    public IList<CustomerMessageCustomizeListItem> MessageCustomizeList { get; set; } = new List<CustomerMessageCustomizeListItem>();
}
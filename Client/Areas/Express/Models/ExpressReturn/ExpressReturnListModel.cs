using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressReturn;

public class ExpressReturnListModel {
    [JsonProperty("expressReturnList")]
    public IList<ExpressReturnListItem> ExpressReturnList { get; set; } = new List<ExpressReturnListItem>();

    [JsonProperty("expressReturnCount")]
    public int ExpressReturnCount { get; set; }

    [JsonProperty("lastId")]
    public int LastId { get; set; }
}
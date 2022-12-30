using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressCheckin;

public class ExpressCheckinListModel
{
    [JsonProperty("expressWaitOrderList")]
    public IList<ExpressCheckinListItem> ExpressWaitOrderList = new List<ExpressCheckinListItem>();

    [JsonProperty("expressCheckinList")]
    public IList<ExpressCheckinListItem> ExpressCheckinList = new List<ExpressCheckinListItem>();

    [JsonProperty("expressWaitOrderCount")]
    public int ExpressWaitOrderCount { get; set; }

    [JsonProperty("expressCheckinCount")]
    public int ExpressCheckinCount { get; set; }

    [JsonProperty("expressWaitOrderTotalCount")]
    public int ExpressWaitOrderTotalCount { get; set; }

    [JsonProperty("expressCheckinTotalCount")]
    public int ExpressCheckinTotalCount { get; set; }    

    [JsonProperty("expressTotalCount")]
    public int ExpressTotalCount { get; set; }
}
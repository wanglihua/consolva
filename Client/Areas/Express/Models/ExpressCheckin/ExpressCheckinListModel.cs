using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressCheckin;

public class ExpressCheckinListModel
{
    [JsonProperty("expressCheckinList")]
    public IList<ExpressCheckinListItem> ExpressCheckinList = new List<ExpressCheckinListItem>();

    [JsonProperty("expressCheckinCount")]
    public int ExpressCheckinCount { get; set; }
}
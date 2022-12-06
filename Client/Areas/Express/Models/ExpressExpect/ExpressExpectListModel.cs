using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressExpect;

public class ExpressExpectListModel
{
    [JsonProperty("expressExpectCount")]
    public int ExpressExpectCount { get; set; }

    [JsonProperty("expressExpectList")]
    public IList<ExpressExpectListItem> ExpressExpectList { get; set; } = new List<ExpressExpectListItem>();
}
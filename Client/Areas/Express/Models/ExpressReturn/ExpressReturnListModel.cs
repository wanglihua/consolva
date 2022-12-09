using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressReturn;

public class ExpressReturnListModel
{
    [JsonProperty("returnExpressList")]
    public IList<ExpressReturnListItem> ReturnExpressList { get; set; } = new List<ExpressReturnListItem>();

    [JsonProperty("returnExpressCount")]
    public int ReturnExpressCount { get; set; }
}
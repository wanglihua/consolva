using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerScoreLog;

public class CustomerScoreLogListModel
{
    [JsonProperty("lastId")]
    public int LastId { get; set; }

    [JsonProperty("logCount")]
    public int LogCount { get; set; }

    [JsonProperty("logList")]
    public IList<CustomerScoreLogListItem> LogList { get; set; } = new List<CustomerScoreLogListItem>();
}
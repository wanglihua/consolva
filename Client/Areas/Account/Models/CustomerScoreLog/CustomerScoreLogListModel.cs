using Newtonsoft.Json;

namespace shunda.Client.Areas.Account.Models.CustomerScoreLog;

public class CustomerScoreLogListModel
{
    [JsonProperty("lastId")]
    public int LastId { get; set; }
}
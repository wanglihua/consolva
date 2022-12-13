using Newtonsoft.Json;

namespace shunda.Client.Areas.Account.Models.CustomerScoreLog;

public class CustomerScoreLogListParam
{
    [JsonProperty("lastId")]
    public int LastId { get; set; }
}
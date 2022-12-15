using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerScoreLog;

public class CustomerScoreLogListParam
{
    [JsonProperty("lastId")]
    public int LastId { get; set; }
}
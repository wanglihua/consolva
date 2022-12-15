using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerScore;

public class CustomerScoreToAccountModel
{
    [JsonProperty("score")]
    public decimal Score { get; set; }
}
using Newtonsoft.Json;

namespace shunda.Client.Areas.Account.Models.CustomerScore;

public class CustomerScoreIndexModel
{
    [JsonProperty("totalScore")]
    public string TotalScore { get; set; } = string.Empty;

    [JsonProperty("validScore")]
    public string ValidScore { get; set; } = string.Empty;

    [JsonProperty("vipRank")]
    public string VipRank { get; set; } = string.Empty;
}
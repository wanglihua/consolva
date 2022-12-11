using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressClaim;

public class ExpressClaimMatchModel
{
    [JsonProperty("match")]
    public bool Match { get; set; }
}
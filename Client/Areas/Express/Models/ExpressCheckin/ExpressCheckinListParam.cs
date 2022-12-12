using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressCheckin;

public class ExpressCheckinListParam
{
    [JsonProperty("query")]
    public string? Query { get; set; }
}
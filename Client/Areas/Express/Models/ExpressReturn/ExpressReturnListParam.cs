using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressReturn;

public class ExpressReturnListParam {
    [JsonProperty("query")]
    public string? Query { get; set; }
}
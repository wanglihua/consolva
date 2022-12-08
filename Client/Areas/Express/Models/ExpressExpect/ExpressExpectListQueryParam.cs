using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressExpect;

public class ExpressExpectListQueryParam
{
    [JsonProperty("trackingNoQuery")]
    public string? TrackingNoQuery { get; set; }
}
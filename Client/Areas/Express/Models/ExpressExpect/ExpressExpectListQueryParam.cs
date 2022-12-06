using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressExpect;

public class ExpressExpectListQueryParam
{
    [JsonProperty("trackingNo")]
    public string? TrackingNoQuery { get; set; }
}
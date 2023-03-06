using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressExpect;

public class ExpressExpectAddModel {
    [JsonProperty("claim")]
    public bool Claim { get; set; }
}
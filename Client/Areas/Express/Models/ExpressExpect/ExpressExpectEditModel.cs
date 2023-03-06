using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressExpect;

public class ExpressExpectEditModel {
    [JsonProperty("claim")]
    public bool Claim { get; set; }
}
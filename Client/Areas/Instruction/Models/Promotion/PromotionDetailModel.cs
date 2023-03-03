using Newtonsoft.Json;

namespace shunda.Client.Areas.Instruction.Models.Promotion; 

public class PromotionDetailModel {
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}
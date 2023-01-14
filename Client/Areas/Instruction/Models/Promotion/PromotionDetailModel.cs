using Newtonsoft.Json;

namespace shunda.Client.Areas.Instruction.Models.Promotion; 

public class PromotionDetailModel {
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;
}
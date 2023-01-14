using Newtonsoft.Json;

namespace shunda.Client.Areas.Instruction.Models.UseAgreement; 

public class UseAgreementDetailModel {
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;
}
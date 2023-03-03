using Newtonsoft.Json;

namespace shunda.Client.Areas.Instruction.Models.OrderProcess; 

public class OrderProcessDetailModel {
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}
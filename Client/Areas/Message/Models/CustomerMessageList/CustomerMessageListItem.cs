using Newtonsoft.Json;

namespace shunda.Client.Areas.Message.Models.CustomerMessageList;

public class CustomerMessageListItem
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("time")]
    public string Time { get; set; } = string.Empty;

    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;
}
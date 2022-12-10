using Newtonsoft.Json;

namespace shunda.Client.Areas.Message.Models.CustomerMessageCustomize;

public class CustomerMessageCustomizeListItem
{
    [JsonProperty("typeId")]
    public int TypeId { get; set; }

    [JsonProperty("send")]
    public bool Send { get; set; }

    [JsonProperty("sendHint")]
    public string SendHint { get; set; } = string.Empty;

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
}
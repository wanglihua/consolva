using Newtonsoft.Json;

namespace shunda.Client.Models.Home;

public class HomeClientImageItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonProperty("linkUrl")]
    public string LinkUrl { get; set; } = string.Empty;
}
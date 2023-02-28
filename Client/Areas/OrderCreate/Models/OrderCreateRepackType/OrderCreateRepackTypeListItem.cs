using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.OrderCreateRepackType;

public class OrderCreateRepackTypeListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
}
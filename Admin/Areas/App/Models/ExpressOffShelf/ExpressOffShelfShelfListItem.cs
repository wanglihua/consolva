using Newtonsoft.Json;

namespace shunda.Admin.Areas.App.Models.ExpressOffShelf;

public class ExpressOffShelfShelfListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("expressCount")]
    public int ExpressCount { get; set; }
}
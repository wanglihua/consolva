using Newtonsoft.Json;

namespace shunda.Admin.Areas.App.Models.ExpressOffShelf;

public class ExpressOffShelfWarehouseListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}
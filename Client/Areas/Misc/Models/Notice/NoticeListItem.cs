using Newtonsoft.Json;

namespace shunda.Client.Areas.Misc.Models.Notice;

public class NoticeListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    [JsonProperty("date")]
    public string Date { get; set; } = string.Empty;

    [JsonProperty("important")]
    public bool Important { get; set; }
}
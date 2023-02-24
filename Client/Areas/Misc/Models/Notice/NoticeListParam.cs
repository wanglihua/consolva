using Newtonsoft.Json;

namespace shunda.Client.Areas.Misc.Models.Notice;

public class NoticeListParam {
    [JsonProperty("lastId")]
    public int LastId { get; set; }
}
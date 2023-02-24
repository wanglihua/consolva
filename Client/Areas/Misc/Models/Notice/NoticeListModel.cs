using Newtonsoft.Json;

namespace shunda.Client.Areas.Misc.Models.Notice;

public class NoticeListModel {
    [JsonProperty("lastId")]
    public int LastId { get; set; }

    [JsonProperty("noticeList")]
    public IList<NoticeListItem> NoticeList { get; set; } = new List<NoticeListItem>();
}
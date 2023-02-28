using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.OrderCreateRepackType;

public class OrderCreateRepackTypeListModel {
    [JsonProperty("repackTypeList")]
    public IList<OrderCreateRepackTypeListItem> RepackTypeList { get; set; } = new List<OrderCreateRepackTypeListItem>();

    [JsonProperty("selectRepackTypeId")]
    public int SelectRepackTypeId { get; set; }

    [JsonProperty("remark")]
    public string Remark { get; set; } = string.Empty;
}
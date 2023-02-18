using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressExpect;

public class ExpressExpectListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("trackingNo")]
    public string TrackingNo { get; set; } = string.Empty;

    [JsonProperty("goodsValue")]
    public string GoodsValue { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 状态名称
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 添加时间
    /// </summary>
    [JsonProperty("addTime")]
    public string AddTime { get; set; } = string.Empty;

    /// <summary>
    /// 对应的快递单号id
    /// </summary>
    [JsonProperty("expressId")]
    public int? ExpressId { get; set; }
}
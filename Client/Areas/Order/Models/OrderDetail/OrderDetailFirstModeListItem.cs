using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailFirstModeListItem {
    /// <summary>
    /// 物流类型
    /// </summary>
    [JsonProperty("modeType")]
    public string ModeType { get; set; } = string.Empty;

    /// <summary>
    /// 发货渠道
    /// </summary>
    [JsonProperty("channel")]
    public string Channel { get; set; } = string.Empty;

    /// <summary>
    /// 物流名称
    /// </summary>
    [JsonProperty("modeName")]
    public string ModeName { get; set; } = string.Empty;
}
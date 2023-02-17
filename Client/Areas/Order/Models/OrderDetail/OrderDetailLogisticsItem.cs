using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailLogisticsItem {
    [JsonProperty("firstModeList")]
    public IList<OrderDetailLogisticsFirstModeItem> FirstModeList { get; set; } = new List<OrderDetailLogisticsFirstModeItem>();
}

public class OrderDetailLogisticsFirstModeItem {
    /// <summary>
    /// 物流类型
    /// </summary>
    [JsonProperty("modeType")]
    public string ModeType { get; set; } = string.Empty;

    /// <summary>
    /// 物流名称
    /// </summary>
    [JsonProperty("modeName")]
    public string ModeName { get; set; } = string.Empty;
}
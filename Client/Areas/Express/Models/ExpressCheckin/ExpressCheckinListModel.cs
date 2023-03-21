using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressCheckin;

public class ExpressCheckinListModel {
    /// <summary>
    /// 需要添加货值的快递单列表
    /// </summary>
    [JsonProperty("expressGoodsValueList")]
    public IList<ExpressCheckinListItem> ExpressGoodsValueList { get; set; } = new List<ExpressCheckinListItem>();

    [JsonProperty("expressWaitOrderList")]
    public IList<ExpressCheckinListItem> ExpressWaitOrderList { get; set; } = new List<ExpressCheckinListItem>();

    [JsonProperty("expressCheckinList")]
    public IList<ExpressCheckinListItem> ExpressCheckinList { get; set; } = new List<ExpressCheckinListItem>();

    /// <summary>
    /// 需要添加货值的快递单数量
    /// </summary>
    [JsonProperty("expressGoodsValueCount")]
    public int ExpressGoodsValueCount { get; set; }

    [JsonProperty("expressWaitOrderCount")]
    public int ExpressWaitOrderCount { get; set; }

    [JsonProperty("expressCheckinCount")]
    public int ExpressCheckinCount { get; set; }

    [JsonProperty("expressWaitOrderTotalCount")]
    public int ExpressWaitOrderTotalCount { get; set; }

    [JsonProperty("expressCheckinTotalCount")]
    public int ExpressCheckinTotalCount { get; set; }

    [JsonProperty("expressTotalCount")]
    public int ExpressTotalCount { get; set; }

    [JsonProperty("forbiddenCount")]
    public  int ForbiddenCount { get; set; } 
}
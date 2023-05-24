using Newtonsoft.Json;

namespace shunda.Admin.Areas.Express.Models.ExpressGoodsValue;

public class ExpressGoodsValueSaveModel {
    [JsonProperty("expressId")]
    public int ExpressId { get; set; }

    [JsonProperty("expressGoodsValue")]
    public string ExpressGoodsValue { get; set; } = string.Empty;

    [JsonProperty("orderGoodsValue")]
    public string OrderGoodsValue { get; set; } = string.Empty;
}
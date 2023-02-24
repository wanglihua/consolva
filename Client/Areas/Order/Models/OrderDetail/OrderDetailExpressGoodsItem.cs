using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailExpressGoodsItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("goodsName")]
    public string GoodsName { get; set; } = string.Empty;

    [JsonProperty("goodsType")]
    public string GoodsType { get; set; } = string.Empty;

    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    [JsonProperty("unit")]
    public string Unit { get; set; } = string.Empty;
}
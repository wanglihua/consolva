using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressCheckin;

public class ExpressCheckinGoodsListItem
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("goodsName")]
    public string GoodsName { get; set; } = string.Empty;

    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    [JsonProperty("unit")]
    public string Unit { get; set; } = string.Empty;

    [JsonProperty("price")]
    public string Price { get; set; } = string.Empty;
}
using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.FreightEstimateGoods;

public class FreightEstimateGoodsSaveModel {
    [JsonProperty("goodsTypeId")]
    public int GoodsTypeId { get; set; }

    [JsonProperty("goodsValue")]
    public string GoodsValue { get; set; } = string.Empty;

    [JsonProperty("goodsList")]
    public IList<FreightEstimateGoodsListItem> GoodsList { get; set; } = new List<FreightEstimateGoodsListItem>();
}
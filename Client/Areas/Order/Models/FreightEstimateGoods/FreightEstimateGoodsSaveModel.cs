using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.FreightEstimateGoods; 

public class FreightEstimateGoodsSaveModel {
    [JsonProperty("goodsType")]
    public string GoodsType { get; set; } = string.Empty;

    [JsonProperty("totalGoodsValue")]
    public string TotalGoodsValue { get; set; } = string.Empty;

    [JsonProperty("goodsList")]
    public IList<FreightEstimateGoodsListItem> GoodsList { get; set; } = new List<FreightEstimateGoodsListItem>();
}
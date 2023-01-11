using Newtonsoft.Json;
using shunda.Client.Areas.Order.Models.FreightEstimateGoods;

namespace shunda.Client.Areas.Order.Models.FreightEstimateSummary;

public class FreightEstimateSummaryModel {
    [JsonProperty("goodsList")]
    public IList<FreightEstimateGoodsListItem> GoodsList = new List<FreightEstimateGoodsListItem>();

    [JsonProperty("goodsType")]
    public string GoodsType { get; set; } = string.Empty;

    [JsonProperty("goodsValue")]
    public string GoodsValue { get; set; } = string.Empty;

    [JsonProperty("totalPackageCount")]
    public int TotalPackageCount { get; set; }

    [JsonProperty("addressReceive")]
    public FreightEstimateSummaryAddressReceiveItem AddressReceive { get; set; } = new();

    [JsonProperty("firstMode")]
    public FreightEstimateSummaryFirstModeItem FirstMode { get; set; } = new();

    [JsonProperty("gstFee")]
    public string GstFee { get; set; } = string.Empty;
}
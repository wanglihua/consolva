using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.FreightEstimateSummary;

public class FreightEstimateSummaryModel {
    [JsonProperty("goodsList")]
    public IList<FreightEstimateSummaryGoodsListItem> GoodsList = new List<FreightEstimateSummaryGoodsListItem>();

    [JsonProperty("goodsType")]
    public string GoodsType { get; set; } = string.Empty;

    [JsonProperty("goodsValue")]
    public string GoodsValue { get; set; } = string.Empty;

    [JsonProperty("totalPackageCount")]
    public int TotalPackageCount { get; set; }

    [JsonProperty("receiveAddress")]
    public FreightEstimateSummaryAddressReceiveItem ReceiveAddress { get; set; } = new();

    [JsonProperty("firstMode")]
    public FreightEstimateSummaryFirstModeItem FirstMode { get; set; } = new();

    [JsonProperty("feeList")]
    public IList<FreightEstimateSummaryFeeListItem> FeeList { get; set; } = new List<FreightEstimateSummaryFeeListItem>();

    [JsonProperty("gstFee")]
    public string GstFee { get; set; } = string.Empty;
}
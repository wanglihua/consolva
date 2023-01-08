using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderCreateSummary;

public class OrderCreateSummaryModel {
    [JsonProperty("expressList")]
    public IList<OrderCreateSummaryExpressListItem> ExpressList = new List<OrderCreateSummaryExpressListItem>();

    [JsonProperty("goodsType")]
    public string GoodsType { get; set; } = string.Empty;

    [JsonProperty("totalPackageCount")]
    public int TotalPackageCount { get; set; }

    [JsonProperty("totalGoodsValue")]
    public string TotalGoodsValue { get; set; } = string.Empty;

    [JsonProperty("receiveAddress")]
    public OrderCreateSummaryReceiveAddressItem ReceiveAddress { get; set; } = new();

    [JsonProperty("sendAddress")]
    public OrderCreateSummarySendAddressItem SendAddress { get; set; } = new();

    [JsonProperty("firstMode")]
    public OrderCreateSummaryFirstModeItem FirstMode { get; set; } = new();

    [JsonProperty("insurance")]
    public OrderCreateSummaryInsuranceItem Insurance { get; set; } = new();

    [JsonProperty("gst")]
    public OrderCreateSummaryGstItem Gst { get; set; } = new();
}
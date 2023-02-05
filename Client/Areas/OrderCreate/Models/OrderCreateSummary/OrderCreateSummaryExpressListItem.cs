using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.OrderCreateSummary;

public class OrderCreateSummaryExpressListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("trackingNo")]
    public string TrackingNo { get; set; } = string.Empty;

    [JsonProperty("expressCompany")]
    public string ExpressCompany { get; set; } = string.Empty;

    [JsonProperty("checkinTrackingNo")]
    public string CheckinTrackingNo { get; set; } = string.Empty;

    [JsonProperty("goodsType")]
    public string GoodsType { get; set; } = string.Empty;

    [JsonProperty("addDate")]
    public string AddDate { get; set; } = string.Empty;

    [JsonProperty("customerRemark")]
    public string CustomerRemark { get; set; } = string.Empty;

    [JsonProperty("warehouseRemark")]
    public string WarehouseRemark { get; set; } = string.Empty;

    [JsonProperty("serviceRemark")]
    public string ServiceRemark { get; set; } = string.Empty;

    [JsonProperty("goodsCount")]
    public int GoodsCount { get; set; }

    [JsonProperty("goodsList")]
    public IList<OrderCreateSummaryExpressGoodsListItem> GoodsList { get; set; } = new List<OrderCreateSummaryExpressGoodsListItem>();
}
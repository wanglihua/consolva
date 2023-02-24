using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailExpressItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("trackingNo")]
    public string TrackingNo { get; set; } = string.Empty;

    [JsonProperty("checkinTrackingNo")]
    public string CheckinTrackingNo { get; set; } = string.Empty;

    [JsonProperty("goodsType")]
    public string GoodsType { get; set; } = string.Empty;

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("checkinTime")]
    public string CheckinTime { get; set; } = string.Empty;

    [JsonProperty("goodsValue")]
    public string GoodsValue { get; set; } = string.Empty;

    [JsonProperty("expressGoodsList")]
    public IList<OrderDetailExpressGoodsItem> ExpressGoodsList { get; set; } = new List<OrderDetailExpressGoodsItem>();
}
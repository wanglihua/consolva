using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderLogisticsTrack;

public class OrderLogisticsTrackModel {
    [JsonProperty("orderCode")]
    public string OrderCode { get; set; } = string.Empty;

    [JsonProperty("firstModeName")]
    public string FirstModeName { get; set; } = string.Empty;

    [JsonProperty("goodsType")]
    public string GoodsType { get; set; } = string.Empty;

    [JsonProperty("trackingNo")]
    public string TrackingNo { get; set; } = string.Empty;

    [JsonProperty("carrierName")]
    public string CarrierName { get; set; } = string.Empty;

    [JsonProperty("eventList")]
    public IList<OrderLogisticsTrackEventItem> EventList { get; set; } = new List<OrderLogisticsTrackEventItem>();
}
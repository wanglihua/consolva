using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailModel {
    [JsonProperty("order")]
    public OrderDetailOrderItem Order { get; set; } = new OrderDetailOrderItem();

    [JsonProperty("firstMode")]
    public OrderDetailFirstModeItem FirstMode { get; set; } = new OrderDetailFirstModeItem();

    [JsonProperty("receiveAddress")]
    public OrderDetailReceiveAddressItem ReceiveAddress { get; set; } = new OrderDetailReceiveAddressItem();

    [JsonProperty("sendAddress")]
    public OrderDetailSendAddressItem SendAddress { get; set; } = new OrderDetailSendAddressItem();

    [JsonProperty("expressList")]
    public IList<OrderDetailExpressItem> ExpressList { get; set; } = new List<OrderDetailExpressItem>();

    [JsonProperty("orderPackList")]
    public IList<OrderDetailOrderPackItem> OrderPackList { get; set; } = new List<OrderDetailOrderPackItem>();

    [JsonProperty("hasOrderPackData")]
    public bool HasOrderPackData { get; set; }

    [JsonProperty("orderPackData")]
    public OrderDetailOrderPackDataItem OrderPackData { get; set; } = new();

    [JsonProperty("hasOrderFee")]
    public bool HasOrderFee { get; set; }

    [JsonProperty("fee")]
    public OrderDetailFeeItem Fee { get; set; } = new();
}
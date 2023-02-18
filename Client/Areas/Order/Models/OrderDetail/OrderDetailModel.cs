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
}
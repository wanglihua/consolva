using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailModel {
    [JsonProperty("order")]
    public OrderDetailOrderItem Order { get; set; } = new OrderDetailOrderItem();
    
    
}
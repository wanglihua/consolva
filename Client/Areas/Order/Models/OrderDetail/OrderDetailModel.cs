using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailModel {
    [JsonProperty("order")]
    public OrderDetailItem Order { get; set; } = new OrderDetailItem();
    
    
}
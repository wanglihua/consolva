using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailReceiveAddressItem {
    [JsonProperty("country")]
    public string Country { get; set; } = string.Empty;

    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonProperty("mobile")]
    public string Mobile { get; set; } = string.Empty;

    [JsonProperty("state")]
    public string State { get; set; } = string.Empty;

    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;

    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("zipcode")]
    public string Zipcode { get; set; } = string.Empty;
}

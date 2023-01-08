using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderCreateSummary;

public class OrderCreateSummarySendAddressItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("mobile")]
    public string Mobile { get; set; } = string.Empty;

    [JsonProperty("province")]
    public string Province { get; set; } = string.Empty;

    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;

    [JsonProperty("area")]
    public string Area { get; set; } = string.Empty;

    [JsonProperty("street")]
    public string Street { get; set; } = string.Empty;

    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("zipcode")]
    public string Zipcode { get; set; } = string.Empty;
}
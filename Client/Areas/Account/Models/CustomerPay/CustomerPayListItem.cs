using Newtonsoft.Json;

namespace shunda.Client.Areas.Account.Models.CustomerPay;

public class CustomerPayListItem
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("typeName")]
    public string TypeName { get; set; } = string.Empty;

    [JsonProperty("amount")]
    public string Amount { get; set; } = string.Empty;

    [JsonProperty("orderCode")]
    public string OrderCode { get; set; } = string.Empty;

    [JsonProperty("payTime")]
    public string PayTime { get; set; } = string.Empty;
}
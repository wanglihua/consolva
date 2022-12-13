using Newtonsoft.Json;

namespace shunda.Client.Areas.Account.Models.CustomerScoreLog;

public class CustomerScoreLogListItem
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("typeName")]
    public string TypeName { get; set; } = string.Empty;

    [JsonProperty("score")]
    public string Score { get; set; } = string.Empty;

    [JsonProperty("amount")]
    public string Amount { get; set; } = string.Empty;

    [JsonProperty("isValidScore")]
    public bool IsValidScore { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("orderId")]
    public int? OrderId { get; set; }

    [JsonProperty("orderCode")]
    public string OrderCode { get; set; } = string.Empty;

    [JsonProperty("orderCustomerId")]
    public int? OrderCustomerId { get; set; }

    [JsonProperty("createTime")]
    public string CreateTime { get; set; } = string.Empty;
}
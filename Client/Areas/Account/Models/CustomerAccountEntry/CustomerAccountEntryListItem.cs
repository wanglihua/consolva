using Newtonsoft.Json;

namespace shunda.Client.Areas.Account.Models.CustomerAccountEntry;

public class CustomerAccountEntryListItem
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("changeTypeName")]
    public string ChangeTypeName { get; set; } = string.Empty;

    [JsonProperty("amount")]
    public string Amount { get; set; } = string.Empty;

    [JsonProperty("amountBefore")]
    public string AmountBefore { get; set; } = string.Empty;

    [JsonProperty("amountAfter")]
    public string AmountAfter { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("createTime")]
    public string CreateTime { get; set; } = string.Empty;
}
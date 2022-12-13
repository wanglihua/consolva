using Newtonsoft.Json;

namespace shunda.Client.Areas.Account.Models.CustomerAccount;

public class CustomerAccountIndexModel
{
    [JsonProperty("amount")]
    public string Amount { get; set; } = string.Empty;
}
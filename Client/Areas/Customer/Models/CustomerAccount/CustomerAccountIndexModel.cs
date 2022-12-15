using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerAccount;

public class CustomerAccountIndexModel
{
    [JsonProperty("amount")]
    public string Amount { get; set; } = string.Empty;
}
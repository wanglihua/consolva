using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerAccountEntry;

public class CustomerAccountEntryListParam
{
    [JsonProperty("lastId")]
    public int LastId { get; set; }
}
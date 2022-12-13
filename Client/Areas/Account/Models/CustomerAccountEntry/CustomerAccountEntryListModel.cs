using Newtonsoft.Json;

namespace shunda.Client.Areas.Account.Models.CustomerAccountEntry;

public class CustomerAccountEntryListModel
{
    [JsonProperty("lastId")]
    public int LastId { get; set; }
}
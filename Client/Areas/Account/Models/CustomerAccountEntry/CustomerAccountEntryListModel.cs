using Newtonsoft.Json;

namespace shunda.Client.Areas.Account.Models.CustomerAccountEntry;

public class CustomerAccountEntryListModel
{
    [JsonProperty("lastId")]
    public int LastId { get; set; }

    [JsonProperty("entryCount")]
    public int EntryCount { get; set; }

    [JsonProperty("entryList")]
    public IList<CustomerAccountEntryListItem> EntryList = new List<CustomerAccountEntryListItem>();
}
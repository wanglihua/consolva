using Newtonsoft.Json;

namespace shunda.Client.Areas.Account.Models.CustomerPay;

public class CustomerPayListModel
{
    [JsonProperty("payCount")]
    public int PayCount { get; set; }

    [JsonProperty("payList")]
    public IList<CustomerPayListItem> PayList { get; set; } = new List<CustomerPayListItem>();

    [JsonProperty("lastId")]
    public int LastId { get; set; }    
}
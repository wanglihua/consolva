using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerPay;

public class CustomerPayListParam
{
    [JsonProperty("lastId")]
    public int LastId { get; set; }
}
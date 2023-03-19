using Newtonsoft.Json;
using shunda.Domain.Fee;

namespace shunda.Client.Areas.Customer.Models.CustomerAccountCharge; 

public class CustomerAccountChargeIndexModel {
    [JsonProperty("currencyList")]
    public IList<CurrencyType> CurrencyList { get; set; } = new List<CurrencyType>();
}
using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressCheckinPrice;

public class ExpressCheckinPriceSaveGoodsPriceItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("price")]
    public string Price { get; set; } = string.Empty;
}
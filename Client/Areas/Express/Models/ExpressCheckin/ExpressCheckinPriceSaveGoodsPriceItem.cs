using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressCheckin;

public class ExpressCheckinPriceSaveGoodsPriceItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("price")]
    public string Price { get; set; } = string.Empty;
}
using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressCheckinPrice;

public class ExpressCheckinPriceSaveParam {
    [JsonProperty("expressId")]
    public int ExpressId { get; set; }

    [JsonProperty("goodsPriceList")]
    public IList<ExpressCheckinPriceSaveGoodsPriceItem> GoodsPriceList = new List<ExpressCheckinPriceSaveGoodsPriceItem>();
}
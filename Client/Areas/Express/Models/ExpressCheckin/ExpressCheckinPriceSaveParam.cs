using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressCheckin;

public class ExpressCheckinPriceSaveParam {
    [JsonProperty("expressId")]
    public int ExpressId { get; set; }

    [JsonProperty("goodsPriceList")]
    public IList<ExpressCheckinPriceSaveGoodsPriceItem> GoodsPriceList = new List<ExpressCheckinPriceSaveGoodsPriceItem>();
}
using Newtonsoft.Json;

namespace shunda.Admin.Areas.Express.Models.ExpressAoPinImport;

public class ExpressAoPinImportExpressGoods {
    [JsonProperty("goodsName")]
    public string GoodsName { get; set; } = string.Empty;

    [JsonProperty("goodsTypeId")]
    public int GoodsTypeId { get; set; }

    [JsonProperty("quantity")]
    public virtual int Quantity { get; set; }

    [JsonProperty("unit")]
    public virtual string Unit { get; set; } = string.Empty;
}
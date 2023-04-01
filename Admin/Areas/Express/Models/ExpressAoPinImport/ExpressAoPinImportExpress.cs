using Newtonsoft.Json;

namespace shunda.Admin.Areas.Express.Models.ExpressAoPinImport;

public class ExpressAoPinImportExpress {
    [JsonProperty("trackingNo")]
    public string TrackingNo { get; set; } = string.Empty;

    [JsonProperty("goodsTypeId")]
    public int GoodsTypeId { get; set; }

    [JsonProperty("billCustomerCode")]
    public string BillCustomerCode { get; set; } = string.Empty;

    [JsonProperty("customerId")]
    public int CustomerId { get; set; }

    [JsonProperty("packageCount")]
    public int PackageCount { get; set; }

    [JsonProperty("totalWeight")]
    public decimal TotalWeight { get; set; }

    [JsonProperty("totalVolume")]
    public decimal TotalVolume { get; set; }
}
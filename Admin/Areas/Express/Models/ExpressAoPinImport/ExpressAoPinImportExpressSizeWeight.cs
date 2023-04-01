using Newtonsoft.Json;

namespace shunda.Admin.Areas.Express.Models.ExpressAoPinImport;

public class ExpressAoPinImportExpressSizeWeight {
    [JsonProperty("length")]
    public decimal Length { get; set; }

    [JsonProperty("width")]
    public decimal Width { get; set; }

    [JsonProperty("height")]
    public decimal Height { get; set; }

    [JsonProperty("weight")]
    public decimal Weight { get; set; }

    [JsonProperty("packageCount")]
    public int PackageCount { get; set; }
}
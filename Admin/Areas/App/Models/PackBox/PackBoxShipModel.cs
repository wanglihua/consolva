using Newtonsoft.Json;

namespace shunda.Admin.Areas.App.Models.PackBox;

public class PackBoxShipModel {
    [JsonProperty("packBoxCode")]
    public string PackBoxCode { get; set; } = string.Empty;

    [JsonProperty("orderCode")]
    public string OrderCode { get; set; } = string.Empty;

    [JsonProperty("customerCode")]
    public string CustomerCode { get; set; } = string.Empty;

    [JsonProperty("countryName")]
    public string CountryName { get; set; } = string.Empty;

    [JsonProperty("firstModeName")]
    public string FirstModeName { get; set; } = string.Empty;

    [JsonProperty("arrearsAmount")]
    public string ArrearsAmount { get; set; } = string.Empty;

    [JsonProperty("hasArrears")]
    public bool HasArrears { get; set; }
}
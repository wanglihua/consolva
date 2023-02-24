using Newtonsoft.Json;

namespace shunda.Client.Areas.Instruction.Models.WarehouseAddress;

public class WarehouseAddressDetailModel {
    [JsonProperty("customerCode")]
    public string CustomerCode { get; set; } = string.Empty;

    [JsonProperty("area")]
    public string Area { get; set; } = string.Empty;

    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonProperty("zipcode")]
    public string Zipcode { get; set; } = string.Empty;
}
using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerReceiveAddress;

public class CustomerReceiveAddressListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonProperty("mobile")]
    public string Mobile { get; set; } = string.Empty;

    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;

    [JsonProperty("state")]
    public string State { get; set; } = string.Empty;

    [JsonProperty("zipcode")]
    public string Zipcode { get; set; } = string.Empty;

    [JsonProperty("tfn")]
    public string Tfn { get; set; } = string.Empty;

    [JsonProperty("company")]
    public string Company { get; set; } = string.Empty;

    [JsonProperty("unitLoc")]
    public string UnitLoc { get; set; } = string.Empty;

    [JsonProperty("isDefault")]
    public bool IsDefault { get; set; }
}
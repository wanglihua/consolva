using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerReceiveAddress;

public class CustomerReceiveAddressSaveModel {
    [JsonProperty("id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "[国家]不能为空！")]
    [JsonProperty("countryId")]
    public int? CountryId { get; set; }

    [Required(ErrorMessage = "[first name]不能为空！")]
    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "[last name]不能为空！")]
    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "[手机号码]不能为空！")]
    [JsonProperty("mobile")]
    public string Mobile { get; set; } = string.Empty;

    [Required(ErrorMessage = "[门牌号/路名]不能为空！")]
    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "[city/suburb]不能为空！")]
    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "[州]不能为空！")]
    [JsonProperty("state")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "[邮编]不能为空！")]
    [JsonProperty("zipcode")]
    public string Zipcode { get; set; } = string.Empty;

    [JsonProperty("tfn")]
    public string Tfn { get; set; } = string.Empty;

    [JsonProperty("company")]
    public string Company { get; set; } = string.Empty;

    [JsonProperty("unitLoc")]
    public string UnitLoc { get; set; } = string.Empty;
}
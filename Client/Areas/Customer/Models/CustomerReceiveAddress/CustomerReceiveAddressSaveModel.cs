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
    [MaxLength(60, ErrorMessage = "[first name]长度不能大于60！")]
    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "[last name]不能为空！")]
    [MaxLength(60, ErrorMessage = "[last name]长度不能大于60！")]
    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "[手机号码]不能为空！")]
    [MaxLength(60, ErrorMessage = "[手机号码]长度不能大于60！")]
    [JsonProperty("mobile")]
    public string Mobile { get; set; } = string.Empty;

    [Required(ErrorMessage = "[门牌号/路名]不能为空！")]
    [MaxLength(200, ErrorMessage = "[门牌号/路名]长度不能大于200！")]
    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "[city/suburb]不能为空！")]
    [MaxLength(50, ErrorMessage = "[city/suburb]长度不能大于50！")]
    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "[州]不能为空！")]
    [MaxLength(50, ErrorMessage = "[州]长度不能大于50！")]
    [JsonProperty("state")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "[邮编]不能为空！")]
    [MaxLength(10, ErrorMessage = "[邮编]长度不能大于10！")]
    [JsonProperty("zipcode")]
    public string Zipcode { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "[ABN/ACN]长度不能大于100！")]
    [JsonProperty("tfn")]
    public string Tfn { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "[公司/店铺名]长度不能大于100！")]
    [JsonProperty("company")]
    public string Company { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "[unit号]长度不能大于20！")]
    [JsonProperty("unitLoc")]
    public string UnitLoc { get; set; } = string.Empty;
}
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerSendAddress;

public class CustomerSendAddressSaveModel {
    [JsonProperty("id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "[姓名]不能为空！")]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "[手机号码]不能为空！")]
    [JsonProperty("mobile")]
    public string Mobile { get; set; } = string.Empty;

    [Required(ErrorMessage = "[地区]不能为空！")]
    [JsonProperty("streetId")]
    public int? StreetId { get; set; }

    [Required(ErrorMessage = "[地址详细]不能为空！")]
    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "[邮编]不能为空！")]
    [JsonProperty("zipcode")]
    public string Zipcode { get; set; } = string.Empty;
}
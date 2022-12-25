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

    [Required(ErrorMessage = "[省]不能为空！")]
    [JsonProperty("provinceId")]
    public int ProvinceId { get; set; }

    [Required(ErrorMessage = "[市]不能为空！")]
    [JsonProperty("cityId")]
    public int CityId { get; set; }

    [Required(ErrorMessage = "[区/县]不能为空！")]
    [JsonProperty("areaId")]
    public int AreaId { get; set; }

    [Required(ErrorMessage = "[街道]不能为空！")]
    [JsonProperty("streetId")]
    public int StreetId { get; set; }

    [Required(ErrorMessage = "[地址详细]不能为空！")]
    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "[邮编]不能为空！")]
    [JsonProperty("zipcode")]
    public string Zipcode { get; set; } = string.Empty;
}
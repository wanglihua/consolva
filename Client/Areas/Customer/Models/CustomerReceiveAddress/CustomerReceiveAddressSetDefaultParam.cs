using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Customer.Models.CustomerReceiveAddress;

public class CustomerReceiveAddressSetDefaultParam : CustomerReceiveAddressListParam {
    [JsonProperty("id")]
    [Required(ErrorMessage = "地址id不能为空！")]
    public int Id { get; set; } = 0;
}
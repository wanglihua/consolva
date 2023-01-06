using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderCreateAddressReceive;

public class OrderCreateAddressReceiveSaveParam {
    [Required(ErrorMessage = "收货地址不能为空！")]
    [JsonProperty("addressId")]
    public int AddressId { get; set; }
}
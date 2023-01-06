using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderCreateAddressSend;

public class OrderCreateAddressSendSaveParam {
    [Required(ErrorMessage = "发货地址不能为空！")]
    [JsonProperty("addressId")]
    public int AddressId { get; set; }
}
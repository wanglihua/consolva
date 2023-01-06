using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.FreightEstimateAddressReceive; 

public class FreightEstimateAddressReceiveSaveParam {
    [Required(ErrorMessage = "收货地址不能为空！")]
    [JsonProperty("addressId")]
    public int AddressId { get; set; }
}
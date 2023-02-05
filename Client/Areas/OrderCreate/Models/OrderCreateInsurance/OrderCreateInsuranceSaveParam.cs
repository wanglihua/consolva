using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.OrderCreateInsurance;

public class OrderCreateInsuranceSaveParam {
    [Required(ErrorMessage = "保险方案不能为空！")]
    [JsonProperty("insuranceId")]
    public int InsuranceId { get; set; }
}
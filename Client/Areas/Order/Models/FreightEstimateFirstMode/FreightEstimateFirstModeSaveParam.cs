using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.FreightEstimateFirstMode; 

public class FreightEstimateFirstModeSaveParam {
    [Required(ErrorMessage = "头程物流不能为空！")]
    [JsonProperty("firstModeId")]
    public int FirstModeId { get; set; }
}
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderCreateFirstMode;

public class OrderCreateFirstModeSaveParam {
    [Required(ErrorMessage = "头程物流不能为空！")]
    [JsonProperty("firstModeId")]
    public int FirstModeId { get; set; }
}
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressCheckin;

public class ExpressCheckinRemoveWaitOrderParam : ExpressCheckinListParam
{
    [Required(ErrorMessage = "快递单号不能为空！")]
    [JsonProperty("expressId")]
    public int ExpressId { get; set; }
}
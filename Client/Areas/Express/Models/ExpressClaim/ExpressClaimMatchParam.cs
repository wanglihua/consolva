using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressClaim;

public class ExpressClaimMatchParam
{
    [Required(ErrorMessage = "申领件ID不能为空！")]
    [JsonProperty("expressId")]
    public int ExpressId { get; set; }

    [Required(ErrorMessage = "快递单单号不能为空！")]
    [JsonProperty("trackingNoInput")]
    public string TrackingNoInput { get; set; } = string.Empty;
}
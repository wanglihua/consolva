using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressExpect;

public class ExpressExpectAddModel
{
    [Required(ErrorMessage = "快递单号不能为空！")]
    [MaxLength(30, ErrorMessage = "快递单号长度不能大于30！")]
    [JsonProperty("trackingNo")]
    public string TrackingNo { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "备注长度不能大于500！")]
    [JsonProperty("description")]
    public string? Description { get; set; }
}
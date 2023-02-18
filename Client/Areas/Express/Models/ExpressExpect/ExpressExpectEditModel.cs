using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressExpect;

public class ExpressExpectEditModel {
    [Required(ErrorMessage = "快递单预告id不能为空！")]
    [JsonProperty("id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "快递单号不能为空！")]
    [MaxLength(30, ErrorMessage = "快递单号长度不能大于30！")]
    [JsonProperty("trackingNo")]
    public string TrackingNo { get; set; } = string.Empty;

    [JsonProperty("goodsValue")]
    public string GoodsValue { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "备注长度不能大于500！")]
    [JsonProperty("description")]
    public string? Description { get; set; }
}

// public class ExpressExpectEditModelValidator : 
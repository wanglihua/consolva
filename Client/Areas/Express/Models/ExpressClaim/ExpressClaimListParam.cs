using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressClaim;

public class ExpressClaimListParam
{
    /// <summary>
    /// 快递单号，入库跟踪号查询，将来可能会加上别的字段信息的查询
    /// </summary>
    [MaxLength(ErrorMessage = "查询字符串不能太长！")]
    [JsonProperty("query")]
    public string? Query { get; set; }

    /// <summary>
    /// 前面列表末尾id
    /// </summary>
    [JsonProperty("lastExpressId")]
    public int LastExpressId { get; set; }
}
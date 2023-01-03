using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressReturn;

public class ExpressReturnListParam {
    [JsonProperty("query")]
    public string? Query { get; set; }

    /// <summary>
    /// 前面列表末尾id
    /// </summary>
    [JsonProperty("lastId")]
    public int LastId { get; set; }

    /// <summary>
    /// 用于数据暂存
    /// </summary>
    [BindNever]
    [JsonIgnore]
    public int LoginCustomerId { get; set; }
}
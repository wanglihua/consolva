using shunda.Core.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace shunda.Core;

public abstract class Entity : EntityNoCreateNoUpdate {
    [BindNever()]
    [JsonProperty("createTime")]
    [JsonConverter(typeof(ShortDateTimeConverter))]
    public virtual DateTime CreateTime { get; set; }

    [BindNever()]
    [JsonProperty("updateTime")]
    [JsonConverter(typeof(ShortDateTimeConverter))]
    public virtual DateTime UpdateTime { get; set; }
}